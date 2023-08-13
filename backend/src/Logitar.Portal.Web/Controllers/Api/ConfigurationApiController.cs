using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Configurations.Payloads;
using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Sessions.Models;
using Microsoft.AspNetCore.Mvc;
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
      if (payload.User.Password == null)
      {
        return BadRequest(new { code = "PasswordIsRequired" });
      }

      await _configurationService.InitializeAsync(payload, cancellationToken);

      var signInPayload = new SignInPayload
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
