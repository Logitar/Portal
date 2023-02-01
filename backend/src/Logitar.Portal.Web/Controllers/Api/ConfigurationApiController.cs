using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Configurations.Payloads;
using Logitar.Portal.Contracts.Accounts.Payloads;
using Logitar.Portal.Contracts.Sessions.Models;
using Logitar.Portal.Web.Extensions;
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

      SignInPayload signInPayload = new()
      {
        Username = payload.User.Username,
        Password = payload.User.Password,
        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
        AdditionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers)
      };
      SessionModel session = await _accountService.SignInAsync(signInPayload, realm: null, cancellationToken);
      HttpContext.SignIn(session);

      return NoContent();
    }
  }
}
