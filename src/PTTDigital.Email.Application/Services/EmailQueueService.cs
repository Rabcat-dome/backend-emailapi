using PTTDigital.Email.Application.Repositories;
using PTTDigital.Email.Application.ViewModels.Requests;
using PTTDigital.Email.Application.ViewModels.Responses;
using PTTDigital.Email.Data.Context;
using PTTDigital.Email.Data.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTTDigital.Email.Data.Service;
using PTTDigital.Email.Data.Models;
using Microsoft.Extensions.Logging;
using PTTDigital.Email.Common.ApplicationUser.User;
using PTTDigital.Email.Common.Configuration.AppSetting;
using System.Collections;

namespace PTTDigital.Email.Application.Services
{
    public class EmailQueueService : IEmailQueueService
    {
        private readonly IApplicationUser _applicationUser;
        private readonly ILogger<EmailQueueService> _logger;
        private readonly IEmailDataService _emailDataService;
        private readonly IGenerator _generator;
        private readonly IAppSetting _appSetting;


        public EmailQueueService(IApplicationUser applicationUser, ILogger<EmailQueueService> logger, IEmailDataService emailDataService, IGenerator generator, IAppSetting appSetting)
        {
            _applicationUser = applicationUser;
            _logger = logger;
            _emailDataService = emailDataService;
            _generator = generator;
            _appSetting = appSetting;
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
                    IsTest = _appSetting.IsTest,
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

#pragma warning disable CS4014
            Task.Run(async () => await _emailDataService.SaveChangeAsync()).ConfigureAwait(false);
#pragma warning restore CS4014

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
    }
}
