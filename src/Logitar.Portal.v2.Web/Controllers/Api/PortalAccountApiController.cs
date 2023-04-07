using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers.Api;

/// <summary>
/// TODO(fpion): Accounts
/// </summary>
[ApiController]
[Route("api/portal/account")]
public class PortalAccountApiController : ControllerBase
{
  //private readonly IAccountService _accountService;

  //public PortalAccountApiController(IAccountService accountService)
  //{
  //  _accountService = accountService;
  //}

  //[HttpPost("sign/in")]
  //public async Task<ActionResult> SignInAsync([FromBody] PortalSignInPayload payload, CancellationToken cancellationToken)
  //{
  //  SessionModel session = await _accountService.SignInAsync(new SignInPayload
  //  {
  //    Username = payload.Username,
  //    Password = payload.Password,
  //    Remember = payload.Remember,
  //    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
  //    AdditionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers)
  //  }, realm: null, cancellationToken);
  //  HttpContext.SetSession(session);

  //  return NoContent();
  //}
}
