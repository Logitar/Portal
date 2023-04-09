using Logitar.Portal.v2.Core.Configurations;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers.Api;

/// <summary>
/// TODO(fpion): SignIn
/// </summary>
[ApiController]
[Route("api/configurations")]
public class ConfigurationApiController : ControllerBase
{
  //private readonly IAccountService _accountService;
  private readonly IConfigurationService _configurationService;

  public ConfigurationApiController(/*IAccountService accountService,*/ IConfigurationService configurationService)
  {
    //_accountService = accountService;
    _configurationService = configurationService;
  }

  [HttpPost]
  public async Task<ActionResult> InitializeAsync([FromBody] InitializeConfigurationInput input, CancellationToken cancellationToken)
  {
    if (await _configurationService.IsInitializedAsync(cancellationToken))
    {
      return Forbid();
    }

    Uri url = new($"{Request.Scheme}://{Request.Host}");

    await _configurationService.InitializeAsync(input, url, cancellationToken);

    //SignInPayload signInPayload = new()
    //{
    //  Username = input.User.Username,
    //  Password = input.User.Password,
    //  IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
    //  AdditionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers)
    //};
    //SessionModel session = await _accountService.SignInAsync(signInPayload, realm: null, cancellationToken);
    //HttpContext.SetSession(session);

    return NoContent();
  }
}
