﻿using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers.Api;

/// <summary>
/// TODO(fpion): Configurations
/// </summary>
[ApiController]
[Route("api/configurations")]
public class ConfigurationApiController : ControllerBase
{
  //private readonly IAccountService _accountService;
  //private readonly IConfigurationService _configurationService;

  //public ConfigurationApiController(IAccountService accountService, IConfigurationService configurationService)
  //{
  //  _accountService = accountService;
  //  _configurationService = configurationService;
  //}

  //[HttpPost]
  //public async Task<ActionResult> InitializeAsync([FromBody] InitializeConfigurationPayload payload, CancellationToken cancellationToken)
  //{
  //  if (payload.User.Password == null)
  //  {
  //    return BadRequest(new { code = "PasswordIsRequired" });
  //  }

  //  await _configurationService.InitializeAsync(payload, cancellationToken);

  //  var signInPayload = new SignInPayload
  //  {
  //    Username = payload.User.Username,
  //    Password = payload.User.Password,
  //    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
  //    AdditionalInformation = JsonSerializer.Serialize(HttpContext.Request.Headers)
  //  };
  //  SessionModel session = await _accountService.SignInAsync(signInPayload, realm: null, cancellationToken);
  //  HttpContext.SetSession(session);

  //  return NoContent();
  //}
}