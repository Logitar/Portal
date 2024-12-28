﻿using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.ApiKeys.Commands;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Extensions;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Logitar.Portal.Authentication;

internal class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions;

internal class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
  private readonly IActivityPipeline _activityPipeline;

  public ApiKeyAuthenticationHandler(IActivityPipeline activityPipeline, IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _activityPipeline = activityPipeline;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (Context.Request.Headers.TryGetValue(Headers.ApiKey, out StringValues values))
    {
      string? value = values.Single();
      if (!string.IsNullOrWhiteSpace(value))
      {
        try
        {
          AuthenticateApiKeyPayload payload = new(value);
          AuthenticateApiKeyCommand command = new(payload);
          ApiKeyModel apiKey = await _activityPipeline.ExecuteAsync(command, new ContextParameters());

          Context.SetApiKey(apiKey);

          ClaimsPrincipal principal = new(apiKey.CreateClaimsIdentity(Scheme.Name));
          AuthenticationTicket ticket = new(principal, Scheme.Name);

          return AuthenticateResult.Success(ticket);
        }
        catch (Exception exception)
        {
          return AuthenticateResult.Fail(exception);
        }
      }
    }

    return AuthenticateResult.NoResult();
  }
}
