using Microsoft.AspNetCore.Mvc;
using Logitar.Portal.Core.Accounts;
using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Core.Configurations.Payloads;
using Logitar.Portal.Core.Sessions.Models;
using System.Text.Json;

namespace Logitar.Portal.Web.Controllers.Api
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
      await _configurationService.InitializeAsync(payload, cancellationToken);

      string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
      string? additionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers);

      var signInPayload = new SignInPayload
      {
        Username = payload.User.Username,
        Password = payload.User.Password
      };
      SessionModel session = await _accountService.SignInAsync(signInPayload, realm: null, ipAddress, additionalInformation, cancellationToken);
      HttpContext.SetSession(session);

      return NoContent();
    }
  }
}
