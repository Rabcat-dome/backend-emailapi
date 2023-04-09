﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTTDigital.Email.Api.Constants;
using PTTDigital.Email.Common.ApplicationUser.User;
using PTTDigital.Email.Common.Exception;
using PTTDigital.Email.Common.Helper.LogExtension;

namespace PTTDigital.Email.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/Auth/[controller]")]
public class EmailTriggerController : ControllerBase
{

    private readonly IApplicationUser _applicationUser;
    private readonly ILogger<EmailTriggerController> _logger;

    public EmailTriggerController( IApplicationUser applicationUser,ILogger<EmailTriggerController> logger)
    {
        this._applicationUser = applicationUser;
        _logger = logger;
    }

    /// <summary>
    /// ใช้เพื่อกวาดเมลล์ทุก ๆ x นาทีแล้วส่งไปที่ SMTP  กรณีที่มี Error ใด ๆ จะส่ง PK ของ Queue Id ไปที่ AdminEmail เพื่อแจ้งให้ทราบ (ส่งเข้า AdminMail ทันที)
    /// </summary>
    [HttpPatch]
    [ApiVersion("1.0")]
    [Authorize] //ยังไม่คุมสิทธิ์ไปก่อน  เดี๋ยวขอความชัดเจน
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<IActionResult> RequestSMTP()
    {
        try
        {
            _logger.Log(LogLevel.Debug, LogEvents.Controller, "RequestSMTPJob");

            //var result = await accPolicyService.QueryPagingAsync(pagination);

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

    /// <summary>
    /// ใช้เพื่อกวาดเมลล์ทุก ๆ x นาที ที่อยู่ใน Status ที่อยู่ในเงื่อนไขจบไปแล้วมาไว้ในถัง Archive
    /// </summary>
    [HttpDelete]
    [ApiVersion("1.0")]
    [Authorize] //ยังไม่คุมสิทธิ์ไปก่อน  เดี๋ยวขอความชัดเจน
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<IActionResult> ArchiveMail()
    {
        try
        {
            _logger.Log(LogLevel.Debug, LogEvents.Controller, "ArchiveMailJob");

            //request.CreatedBy = _applicationUser.UserId;

            //await accPolicyService.Add(request, _applicationUser);

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
