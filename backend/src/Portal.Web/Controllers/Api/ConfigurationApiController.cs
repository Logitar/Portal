using Microsoft.AspNetCore.Mvc;
using Portal.Core.Accounts;
using Portal.Core.Accounts.Payloads;
using Portal.Core.Configurations;
using Portal.Core.Configurations.Payloads;
using Portal.Core.Sessions.Models;
using System.Text.Json;

namespace Portal.Web.Controllers.Api
{
  [ApiController]
  [Route("api/configurations")]
  public class ConfigurationApiController : ControllerBase
  {
    private readonly IAccountService _accountService;
    private readonly IConfigurationService _configurationService;

    public ConfigurationApiController(IAccountService accountService, IConfigurationService configurationService)
    {
      _accountService = accountService;
      _configurationService = configurationService;
    }

    [HttpPost]
    public async Task<ActionResult> InitializeAsync([FromBody] InitializeConfigurationPayload payload, CancellationToken cancellationToken)
    {
      var signInPayload = new SignInPayload
      {
        Username = payload.User.Username,
        Password = payload.User.Password
      };

      await _configurationService.InitializeAsync(payload, cancellationToken);

      string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
      string? additionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers);

      SessionModel session = await _accountService.SignInAsync(signInPayload, ipAddress, additionalInformation, cancellationToken);
      HttpContext.SetSession(session);

      return NoContent();
    }
  }
}
