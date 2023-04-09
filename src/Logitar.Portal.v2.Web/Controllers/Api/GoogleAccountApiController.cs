using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers.Api;

/// <summary>
/// TODO(fpion): Accounts
/// </summary>
[ApiController]
//[Authorize(Policy = Constants.Policies.PortalIdentity)] // TODO(fpion): Authorization
[Route("api/realms/{id}/google/account")]
public class GoogleAccountApiController : ControllerBase
{
  //private readonly IGoogleService _googleService;

  //public GoogleAccountApiController(IGoogleService googleService)
  //{
  //  _googleService = googleService;
  //}

  //[HttpPost("auth")]
  //public async Task<ActionResult<SessionModel>> AuthenticateAsync(string id, AuthenticateWithGooglePayload payload, CancellationToken cancellationToken)
  //{
  //  string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
  //  string additionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers);

  //  return Ok(await _googleService.AuthenticateAsync(id, payload, ipAddress, additionalInformation, cancellationToken));
  //}
}
