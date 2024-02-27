﻿using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Constants;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Extensions;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Logitar.Portal.Authentication;

internal class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
  private readonly IApiKeyService _apiKeyService;

  public ApiKeyAuthenticationHandler(IApiKeyService apiKeyService, IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _apiKeyService = apiKeyService;
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
          ApiKey apiKey = await _apiKeyService.AuthenticateAsync(payload);

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
