using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTTDigital.Email.Application.ViewModels.Requests;
using PTTDigital.Email.Application.ViewModels.Responses;
using PTTDigital.Email.Common.ApplicationUser.User;
using PTTDigital.Email.Common.Exception;
using PTTDigital.Email.Common.Helper.LogExtension;

namespace PTTDigital.Email.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/Email/[controller]")]
public class EmailQueueController : ControllerBase
{
    private readonly IApplicationUser _applicationUser;
    private readonly ILogger<EmailQueueController> _logger;

    public EmailQueueController(IApplicationUser applicationUser,ILogger<EmailQueueController> logger)
    {
        this._applicationUser = applicationUser;
        _logger = logger;
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
        try
        {
            _logger.Log(LogLevel.Debug, LogEvents.Controller, queues);

            var userId = _applicationUser.UserId;
            //var result = await accountService.QueryPagingAccountAccPolicyAccGroupAsync(pagination);
            var result = new List<EmailQueueResponse>();

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
        try
        {
            _logger.Log(LogLevel.Debug, LogEvents.Controller, queues);

            //await accountService.AddAccountGroup(request);

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
