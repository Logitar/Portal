using Logitar.Portal.Core2.Accounts;
using Logitar.Portal.Core2.Accounts.Payloads;
using Logitar.Portal.Core2.Configurations;
using Logitar.Portal.Core2.Configurations.Payloads;
using Logitar.Portal.Core2.Sessions.Models;
using Logitar.Portal.Web2.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Logitar.Portal.Web2.Controllers.Api
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

      SignInPayload signInPayload = new()
      {
        Username = payload.User.Username,
        Password = payload.User.Password,
        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
        AdditionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers)
      };
      SessionModel session = await _accountService.SignInAsync(signInPayload, realm: null, cancellationToken);
      HttpContext.SetSession(session);

      return NoContent();
    }
  }
}
