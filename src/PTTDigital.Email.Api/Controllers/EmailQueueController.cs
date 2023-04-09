using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTTDigital.Email.Common.ApplicationUser.User;
using PTTDigital.Email.Common.Exception;

//using PTTDigital.Authentication.Application.ViewModels.Requests.AccountGroup;
//using PTTDigital.Authentication.Application.ViewModels.Responses.AccountGroup;

namespace PTTDigital.Email.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/Email/[controller]")]
public class EmailQueueController : ControllerBase
{
    private readonly IApplicationUser applicationUser;

    public EmailQueueController(IApplicationUser applicationUser)
    {
        this.applicationUser = applicationUser;
    }

    /// <summary>
    /// 1.ดำเนินการเพิ่มรายการที่จะส่งเมลล์  โดยรองรับเป็น List ของ Queue แต่ถ้าต้นทางจะส่งมาแค่รายการเมลล์เดียวก็จะต้องส่ง
    /// 2.จะ reponse 202 กลับออกไปเป็น List ของ Id Map กับ Subject/EmailTo เพื่อเอาไปให้บริหารต่อ เผื่อจะยิง Cancel กลับมา => จะทำการ gen Ulid ที่ใช้ในการ Map ตัวแปรแล้ว return ออกไปก่อนโดยไม่แต่ Database
    /// </summary>
    [HttpPost]
    [ApiVersion("1.0")]
    [Authorize] //ยังไม่คุมสิทธิ์ไปก่อน  เดี๋ยวขอความชัดเจน
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponsePagination<AccGroupResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<IActionResult> InsertEmailQueue([FromBody] Pagination<AccountRequest> pagination)
    {
        try
        {
            //_logger.Log(LogLevel.Debug, LogEvents.Controller, authorization);

            var result = await accountService.QueryPagingAccountAccPolicyAccGroupAsync(pagination);

            return StatusCode(StatusCodes.Status200OK, result);
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
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<IActionResult> CancelEmailQueue([FromBody] AccountGroupRequest request)
    {
        try
        {
            //_logger.Log(LogLevel.Debug, LogEvents.Controller, authorization);

            request.CreatedBy = applicationUser.UserId;

            await accountService.AddAccountGroup(request);

            return StatusCode(StatusCodes.Status201Created);
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
