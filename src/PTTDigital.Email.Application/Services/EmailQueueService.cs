using PTTDigital.Email.Application.ViewModels.Requests;
using PTTDigital.Email.Application.ViewModels.Responses;
using PTTDigital.Email.Data.Service;
using PTTDigital.Email.Data.Models;
using Microsoft.Extensions.Logging;
using PTTDigital.Email.Common.ApplicationUser.User;
using PTTDigital.Email.Common.Configuration.AppSetting;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using PTTDigital.Email.Common.EncryptDecrypt.Cryptography;

namespace PTTDigital.Email.Application.Services
{
    public class EmailQueueService : IEmailQueueService
    {
        private readonly IApplicationUser _applicationUser;
        private readonly ILogger<EmailQueueService> _logger;
        private readonly IEmailDataService _emailDataService;
        private readonly IEmailTriggerService _emailTriggerService;
        private readonly IGenerator _generator;
        private readonly IAppSetting _appSetting;
        private readonly IEncryptDecryptHelper _encryptDecryptHelper;

        public EmailQueueService(IApplicationUser applicationUser, ILogger<EmailQueueService> logger, IEmailDataService emailDataService, IEmailTriggerService emailTriggerService, IGenerator generator, IAppSetting appSetting, IEncryptDecryptHelper encryptDecryptHelper)
        {
            _applicationUser = applicationUser;
            _logger = logger;
            _emailDataService = emailDataService;
            _emailTriggerService = emailTriggerService;
            _generator = generator;
            _appSetting = appSetting;
            _encryptDecryptHelper = encryptDecryptHelper;
        }

        public List<EmailQueueResponse> InsertQueue(List<EmailQueueRequest> queues)
        {
            var userId = _applicationUser.UserId;
            var result = new List<EmailQueueResponse>();
            foreach (var queue in queues)
            {
                var queueId = _generator.GenerateUlid();
                var messageId = _generator.GenerateUlid();

                _logger.LogTrace($"queueId:{queueId} => messageId:{messageId}");

                var queueEmail = new EmailQueue()
                {
                    QueueId = queueId,
                    EmailFrom = queue.EmailFrom,
                    EmailTo = queue.EmailTo,
                    EmailCc = queue.EmailCc ?? string.Empty,
                    RefAccPolicyId = userId,
                    //กรณ๊ Deploy มาแล้วเป็น Centralized เราสามารถยิง Test แยกรายครั้งได้
                    IsTest = _appSetting.IsTest ? _appSetting.IsTest : queue.IsTest,
                };
                var message = new Message()
                {
                    MessageId = messageId,
                    EmailSubject = queue.Subject,
                    EmailBody = queue.Body,
                };
                _emailDataService.EmailQueueRepository.Add(queueEmail);
                _emailDataService.MessageRepository.Add(message);

                result.Add(new EmailQueueResponse()
                {
                    QueueId = queueId,
                    Subject = queue.Subject,
                    EmailTo = queue.EmailTo
                });
            }

            _emailDataService.SaveChangeAsync().ConfigureAwait(false);
            return result;
        }

        public async Task CancelQueue(List<CancelEmailQueueRequest> queueIds)
        {
            //_emailDataService.EmailQueueRepository.Query(x=>queue.Contain(x.QueueId)) => แบบนี้ Performance น่าจะไม่ดี
            var newMessages = _emailDataService.EmailQueueRepository.Query(x => x.Status == QueueStatus.New).ToList();
            foreach (var message in newMessages)
            {
                if (queueIds.Any(x => x.QueueId == message.QueueId))
                {
                    message.Status = QueueStatus.Canceled;
                    _emailDataService.EmailQueueRepository.Update(message);
                }
            }
            await _emailDataService.SaveChangeAsync();
            _logger.LogTrace($"CancelQueue: {queueIds}");
        }

        //Method นี้จะถูกเรียกโดย Job เป็น infinite Loop ดังนั้นถ้าใช้ Exception ต้องมีวิธีจัดการ
        public void TriggerMail()
        {
            var newMailsAsync = _emailDataService.EmailQueueRepository.Query(x => x.Status == QueueStatus.New).ToListAsync();
            List<MailAddress> adminMail = _emailTriggerService.ConvertToMailAddresses(_appSetting.AdminEmail ?? string.Empty);

            _logger.LogTrace($"AdminMail CountBeforeCheck {adminMail.Count}");
            foreach (var mail in adminMail.ToList())
            {
                if (!_emailTriggerService.IsValidEmailAddressFormat(mail.Address))
                {
                    adminMail.Remove(mail);
                    _logger.LogWarning($"AdminEmail {mail.Address} is incorrect Format");
                }
            }
            if (!adminMail.Any())
            {
                _logger.LogError($"Please Assign Admin Email");
                throw new FormatException("No Valid AdminEmail, Please Assign in AppConfig");
            }
            _logger.LogTrace($"AdminMail CountAfterCheck {adminMail.Count}");

            var strMailFrom = _emailTriggerService.ConvertToMailAddresses(_appSetting.DefaultMail ?? string.Empty).FirstOrDefault();
            if (!_emailTriggerService.IsValidEmailAddressFormat(strMailFrom?.Address ?? string.Empty))
            {
                _logger.LogError($"MailFrom in AppConfig =>{strMailFrom?.Address} is incorrect Format");
                throw new FormatException("No Valid mailFrom, Please Assign in AppConfig");
            }

            var newMails = newMailsAsync.Result;
            //ท่อนนี้ช่วยดู SQL Profiler ให้ด้วยครับ
            var messagesLst = _emailDataService.MessageRepository.Query(x => newMails.Any(y => y.MessageId == x.MessageId)).ToListAsync();

            if (!newMails.Any())
            {
                return;
            }
            newMails.ForEach(x => x.Status = QueueStatus.Queueing);
            _emailDataService.EmailQueueRepository.UpdateRange(newMails);

            //Status To Queuing ป้องกันเคสที่มี Trigger รอบใหม่มาติด ๆ กันจะไม่ query ซ้ำ
            var syncState = _emailDataService.SaveChangeAsync();

            foreach (var mail in newMails)
            {
                mail.Status = QueueStatus.Sending;
                _emailDataService.EmailQueueRepository.Update(mail);
#pragma warning disable CS4014
                _ = syncState.Result;
                _emailDataService.SaveChangeAsync().ConfigureAwait(false);
#pragma warning restore CS4014

                var mailTo = _emailTriggerService.ConvertToMailAddresses(mail.EmailTo);
                var mailCc = _emailTriggerService.ConvertToMailAddresses(mail.EmailCc);
                if (string.IsNullOrWhiteSpace(mail.EmailFrom)) mail.EmailFrom = _appSetting.DefaultMailDisplay ?? string.Empty;

                //Mail อะไรที่ส่งไม่ได้ก็จะลง Log ไว้ให้
                foreach (var mailx in mailTo.ToList())
                {
                    if (!_emailTriggerService.IsValidEmailAddressFormat(mailx.Address))
                    {
                        mailTo.Remove(mailx);
                        _logger.LogWarning($"MailTo {mailx.Address} is incorrect Format");
                    }
                }

                foreach (var mailx in mailCc.ToList())
                {
                    if (!_emailTriggerService.IsValidEmailAddressFormat(mailx.Address))
                    {
                        mailCc.Remove(mailx);
                        _logger.LogWarning($"MailCc {mailx.Address} is incorrect Format");
                    }
                }

                var msg = (messagesLst.Result).FirstOrDefault(x => x.MessageId == mail.MessageId);

                if (_emailTriggerService.Validate(mailTo, mailCc, mail.EmailFrom, msg?.EmailSubject ?? string.Empty, msg?.EmailBody ?? string.Empty))
                {
                    try
                    {
                        _emailTriggerService.SendMail(strMailFrom!.Address!, mail.EmailFrom, msg!.EmailSubject!,
                            msg!.EmailBody!, mail.IsHtmlFormat, null, _appSetting.IsTest ? adminMail : mailTo, mailCc);
                        mail.Status = QueueStatus.Completed;
                        mail.Sent = DateTime.Now;
                        mail.IsTest = true;
                        _emailDataService.EmailQueueRepository.Update(mail);
                        _ = _emailDataService.SaveChangeAsync().Result;
                    }
                    catch (SmtpException exception)
                    {
                        if (mail.RetryCount < 5)
                        {
                            //ปรับ Status เพื่อเตรียมส่งใหม่
                            mail.Status = QueueStatus.New;
                            mail.RetryCount += 1;
                        }
                        else
                        {
                            mail.Status = QueueStatus.Failed;
                        }

                        _emailDataService.EmailQueueRepository.Update(mail);
                        _ = _emailDataService.SaveChangeAsync().Result;
                        _logger.LogError($"Send Mail Error Subject:{msg?.EmailSubject} To:{mailTo}");
                        //ฝากแก้ HardCode ด้วยครับ
                        _emailTriggerService.SendMail(strMailFrom!.Address!, mail.EmailFrom,
                            $"PPE EmailAPI Smtp Error",
                            $"Send Mail Error Subject:{msg?.EmailSubject} To:{mailTo}\nQueueId:{mail.QueueId} MessageId:{mail.MessageId}\nException:{exception.Message}"
                            , false, null, adminMail, new List<MailAddress>());
                    }
                }
                else
                {
                    mail.Status = QueueStatus.Failed;
                    _emailDataService.EmailQueueRepository.Update(mail);
                    _ = _emailDataService.SaveChangeAsync().Result;
                    _logger.LogError($"Validate Mail Error Subject:{msg?.EmailSubject} To:{mailTo}");
                    //ฝากแก้ HardCode ด้วยครับ
                    _emailTriggerService.SendMail(strMailFrom!.Address!, mail.EmailFrom,
                        $"PPE EmailAPI ValidateMail Error",
                        $"Validate Mail Error Subject:{msg?.EmailSubject} To:{mailTo}\nQueueId:{mail.QueueId} MessageId:{mail.MessageId}"
                        , false, null, adminMail, new List<MailAddress>());
                }
            }
        }

        public void ArchiveMail()
        {
            var doneMailsAsync = _emailDataService.EmailQueueRepository.Query(x => (int)x.Status > 2).ToListAsync();

            var doneMails = doneMailsAsync.Result;
            foreach (var doneMail in doneMails)
            {
                var archiveId = _generator.GenerateUlid();
                var archiveMail = new EmailArchive()
                {
                    ArchiveId = archiveId,
                    QueueId = doneMail.QueueId,
                    EmailFrom = doneMail.EmailFrom,
                    EmailTo = _encryptDecryptHelper.Encrypt(doneMail.EmailTo, _appSetting.SymmetricKey ?? string.Empty),
                    EmailCc = _encryptDecryptHelper.Encrypt(doneMail.EmailCc, _appSetting.SymmetricKey ?? string.Empty),
                    Initiated = doneMail.Initiated,
                    Sent = doneMail.Sent,
                    IsHtmlFormat = doneMail.IsHtmlFormat,
                    RetryCount = doneMail.RetryCount,
                    Status = doneMail.Status,
                    RefAccPolicyId = doneMail.RefAccPolicyId,
                    IsTest = doneMail.IsTest,
                    MessageId = doneMail.MessageId,
                };
                _emailDataService.EmailArchiveRepository.Update(archiveMail);
            }

            _emailDataService.EmailQueueRepository.RemoveRange(doneMails);
            _ = _emailDataService.SaveChangeAsync().Result;
        }
    }
}
