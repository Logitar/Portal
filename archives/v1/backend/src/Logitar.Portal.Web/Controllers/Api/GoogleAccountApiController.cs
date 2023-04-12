using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Sessions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Logitar.Portal.Web.Controllers.Api
{
  [ApiController]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("api/realms/{id}/google/account")]
  public class GoogleAccountApiController : ControllerBase
  {
    private readonly IGoogleService _googleService;

    public GoogleAccountApiController(IGoogleService googleService)
    {
      _googleService = googleService;
    }

    [HttpPost("auth")]
    public async Task<ActionResult<SessionModel>> AuthenticateAsync(string id, AuthenticateWithGooglePayload payload, CancellationToken cancellationToken)
    {
      string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
      string additionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers);

      return Ok(await _googleService.AuthenticateAsync(id, payload, ipAddress, additionalInformation, cancellationToken));
    }
  }
}
