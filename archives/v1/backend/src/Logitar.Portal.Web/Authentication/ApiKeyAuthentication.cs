using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Application.Claims;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Sessions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Logitar.Portal.Web.Authentication
{
  internal class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
  {
  }

  internal class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
  {
    private readonly IApiKeyQuerier _apiKeyQuerier;
    private readonly IPasswordService _passwordService;
    private readonly ISessionQuerier _sessionQuerier;

    public ApiKeyAuthenticationHandler(
      IApiKeyQuerier apiKeyQuerier,
      IPasswordService passwordService,
      ISessionQuerier sessionQuerier,
      IOptionsMonitor<ApiKeyAuthenticationOptions> options,
      ILoggerFactory logger,
      UrlEncoder encoder,
      ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
      _apiKeyQuerier = apiKeyQuerier;
      _passwordService = passwordService;
      _sessionQuerier = sessionQuerier;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      ApiKeyAuthenticateResult result = await AuthenticateApiKeyAsync();

      if (result.ApiKey != null)
      {
        AuthenticateResult sessionResult = await AuthenticateSessionAsync();
        if (!sessionResult.None)
        {
          return sessionResult;
        }
      }

      return result.Result;
    }

    private async Task<ApiKeyAuthenticateResult> AuthenticateApiKeyAsync()
    {
      if (Context.Request.Headers.TryGetValue(Constants.Headers.ApiKey, out StringValues values))
      {
        try
        {
          string value = values.Single();
          var secureToken = SecureToken.Parse(value);

          ApiKey? apiKey = await _apiKeyQuerier.GetAsync(secureToken.Id, readOnly: true);
          if (apiKey == null)
          {
            return new(AuthenticateResult.Fail($"The API key 'Id={secureToken.Id}' could not be found."));
          }
          else if (apiKey.IsExpired)
          {
            return new(AuthenticateResult.Fail($"The API key 'Id={secureToken.Id}' is expired."));
          }
          else if (!_passwordService.IsMatch(apiKey.KeyHash, secureToken.Key))
          {
            return new(AuthenticateResult.Fail($"The secret did not match for the API key 'Id={secureToken.Id}'."));
          }

          if (!Context.SetApiKey(apiKey))
          {
            throw new InvalidOperationException("The API key context item could not be set.");
          }

          var principal = new ClaimsPrincipal(apiKey.GetClaimsIdentity(Constants.Schemes.ApiKey));
          var ticket = new AuthenticationTicket(principal, Constants.Schemes.ApiKey);

          return new(AuthenticateResult.Success(ticket), apiKey);
        }
        catch (Exception exception)
        {
          return new(AuthenticateResult.Fail(exception));
        }
      }

      return new(AuthenticateResult.NoResult());
    }

    private async Task<AuthenticateResult> AuthenticateSessionAsync()
    {
      if (Context.Request.Headers.TryGetValue(Constants.Headers.Session, out StringValues values))
      {
        try
        {
          var id = Guid.Parse(values.Single());
          Session? session = await _sessionQuerier.GetAsync(id, readOnly: true);
          if (session == null)
          {
            return AuthenticateResult.Fail($"The session 'Id={id}' could not be found.");
          }
          else if (!session.IsActive)
          {
            return AuthenticateResult.Fail($"The session 'Id={session.Id}' has ended.");
          }
          else if (session.User == null)
          {
            return AuthenticateResult.Fail($"The User was null for session 'Id={session.Id}'.");
          }
          else if (session.User.IsDisabled)
          {
            return AuthenticateResult.Fail($"The User is disabled for session 'Id={session.Id}'.");
          }

          if (!Context.SetSession(session))
          {
            throw new InvalidOperationException("The Session context item could not be set.");
          }
          if (!Context.SetUser(session.User))
          {
            throw new InvalidOperationException("The User context item could not be set.");
          }

          var principal = new ClaimsPrincipal(session.User!.GetClaimsIdentity(Constants.Schemes.Session));
          var ticket = new AuthenticationTicket(principal, Constants.Schemes.Session);

          return AuthenticateResult.Success(ticket);
        }
        catch (Exception exception)
        {
          return AuthenticateResult.Fail(exception);
        }
      }

      return AuthenticateResult.NoResult();
    }

    internal class ApiKeyAuthenticateResult
    {
      public ApiKeyAuthenticateResult(AuthenticateResult result)
      {
        Result = result ?? throw new ArgumentNullException(nameof(result));

        if (result.Succeeded)
        {
          throw new ArgumentException("The result should be a failure.", nameof(result));
        }
      }
      public ApiKeyAuthenticateResult(AuthenticateResult result, ApiKey apiKey)
      {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        Result = result ?? throw new ArgumentNullException(nameof(result));

        if (!result.Succeeded)
        {
          throw new ArgumentException("The result should be a success.", nameof(result));
        }
      }

      public ApiKey? ApiKey { get; }
      public AuthenticateResult Result { get; } = null!;
    }
  }
}
