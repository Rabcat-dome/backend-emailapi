using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTTDigital.Email.Application.ViewModels.Requests;
using PTTDigital.Email.Application.ViewModels.Responses;
using PTTDigital.Email.Common.ApplicationUser.User;
using PTTDigital.Email.Common.Configuration.AppSetting;
using PTTDigital.Email.Common.Exception;
using PTTDigital.Email.Common.Helper.LogExtension;
using PTTDigital.Email.Data.Models;
using PTTDigital.Email.Data.Service;

namespace PTTDigital.Email.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/Email/[controller]")]
public class EmailQueueController : ControllerBase
{
    private readonly IApplicationUser _applicationUser;
    private readonly ILogger<EmailQueueController> _logger;
    private readonly IEmailDataService _emailDataService;
    private readonly IGenerator _generator;
    private readonly IAppSetting _appSetting;

    public EmailQueueController(IApplicationUser applicationUser, ILogger<EmailQueueController> logger, IEmailDataService emailDataService, IGenerator generator, IAppSetting appSetting)
    {
        this._applicationUser = applicationUser;
        _logger = logger;
        _emailDataService = emailDataService;
        _generator = generator;
        _appSetting = appSetting;
    }

    /// <summary>
    /// 1.ดำเนินการเพิ่มรายการที่จะส่งเมลล์  โดยรองรับเป็น List ของ Queue แต่ถ้าต้นทางจะส่งมาแค่รายการเมลล์เดียวก็จะต้องส่ง
    /// 2.จะ reponse 202 กลับออกไปเป็น List ของ Id Map กับ Subject/EmailTo เพื่อเอาไปให้บริหารต่อ เผื่อจะยิง Cancel กลับมา => จะทำการ gen Ulid ที่ใช้ในการ Map ตัวแปรแล้ว return ออกไปก่อนโดยไม่แต่ Database
    /// </summary>
    [HttpPost]
    [ApiVersion("1.0")]
    [Authorize] //ยังไม่คุมสิทธิ์ไปก่อน  เดี๋ยวขอความชัดเจน
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(List<EmailQueueResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<IActionResult> InsertEmailQueue([FromBody] List<EmailQueueRequest> queues)
    {
        //อยากจะหาน้องมาช่วยทำ Extract Service ให้สวย ๆ ที่ EmailService
        try
        {
            _logger.Log(LogLevel.Debug, LogEvents.Controller, queues);

            var userId = _applicationUser.UserId;
            var result = new List<EmailQueueResponse>();
            foreach (var queue in queues)
            {
                var queueId = _generator.GenerateUlid();
                var messageId = _generator.GenerateUlid();

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

            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (AuthorizationException ex)
        {
            return StatusCode(StatusCodes.Status401Unauthorized, ex.Model);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// 1.ดำเนินการ Update Record ที่จะยกเลิกการส่งเมลล์
    /// 2.จะ reponse 202 Accepted มาเท่านั้นโดยเชื่อไปก่อนว่าสำเร็จ
    /// </summary>
    [HttpPatch]
    [ApiVersion("1.0")]
    [Authorize] //ยังไม่คุมสิทธิ์ไปก่อน  เดี๋ยวขอความชัดเจน
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(List<CancelEmailQueueRequest>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<IActionResult> CancelEmailQueue([FromBody] List<CancelEmailQueueRequest> queues)
    {
        //อยากจะหาน้องมาช่วยทำ Extract Service ให้สวย ๆ ที่ EmailService
        try
        {
            _logger.Log(LogLevel.Debug, LogEvents.Controller, queues);
            
#pragma warning disable CS4014
            Task.Run(async () =>
            {
                //_emailDataService.EmailQueueRepository.Query(x=>queue.Contain(x.QueueId)) => แบบนี้ Performance น่าจะไม่ดี
                var newMessages = _emailDataService.EmailQueueRepository.Query(x => x.Status == QueueStatus.New).ToList();
                foreach (var message in newMessages)
                {
                    if (queues.Any(x => x.QueueId == message.QueueId))
                    {
                        message.Status = QueueStatus.Canceled;
                        _emailDataService.EmailQueueRepository.Update(message);
                    }
                }

                await _emailDataService.SaveChangeAsync().ConfigureAwait(false);
            });
#pragma warning restore CS4014
            
            return StatusCode(StatusCodes.Status202Accepted);
        }
        catch (AuthorizationException ex)
        {
            return StatusCode(StatusCodes.Status401Unauthorized, ex.Model);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
