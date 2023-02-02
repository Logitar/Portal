using Logitar.Portal.Application;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Application.Claims;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Logitar.Portal.Web.Authentication
{
  internal class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
  {
    private readonly IApiKeyQuerier _apiKeyQuerier;
    private readonly IPasswordService _passwordService;
    private readonly IRepository _repository;
    private readonly ISessionQuerier _sessionQuerier;

    public ApiKeyAuthenticationHandler(
      IApiKeyQuerier apiKeyQuerier,
      IPasswordService passwordService,
      IRepository repository,
      ISessionQuerier sessionQuerier,
      IOptionsMonitor<ApiKeyAuthenticationOptions> options,
      ILoggerFactory logger,
      UrlEncoder encoder,
      ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
      _apiKeyQuerier = apiKeyQuerier;
      _passwordService = passwordService;
      _repository = repository;
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
          XApiKey xApiKey = XApiKey.Parse(values.Single() ?? string.Empty);
          ApiKey? apiKey = await _repository.LoadAsync<ApiKey>(xApiKey.Id);
          ApiKeyModel? apiKeyModel = await _apiKeyQuerier.GetAsync(xApiKey.Id);
          if (apiKey == null || apiKeyModel == null)
          {
            return new(AuthenticateResult.Fail($"The API key 'Id={xApiKey.Id}' could not be found."));
          }
          else if (apiKey.IsExpired())
          {
            return new(AuthenticateResult.Fail($"The API key 'Id={xApiKey.Id}' is expired."));
          }
          else if (!_passwordService.IsMatch(apiKey, xApiKey.Secret))
          {
            return new(AuthenticateResult.Fail($"The secret did not match for the API key 'Id={xApiKey.Id}'."));
          }

          if (!Context.SetApiKey(apiKeyModel))
          {
            throw new InvalidOperationException("The API key context item could not be set.");
          }

          ClaimsPrincipal principal = new(apiKeyModel.GetClaimsIdentity(Constants.Schemes.ApiKey));
          AuthenticationTicket ticket = new(principal, Constants.Schemes.ApiKey);

          return new(AuthenticateResult.Success(ticket), apiKeyModel);
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
          string id = values.Single() ?? string.Empty;
          SessionModel? session = await _sessionQuerier.GetAsync(id);
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

          ClaimsPrincipal principal = new(session.User!.GetClaimsIdentity(Constants.Schemes.Session));
          AuthenticationTicket ticket = new(principal, Constants.Schemes.Session);

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
      public ApiKeyAuthenticateResult(AuthenticateResult result, ApiKeyModel apiKey)
      {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        Result = result ?? throw new ArgumentNullException(nameof(result));

        if (!result.Succeeded)
        {
          throw new ArgumentException("The result should be a success.", nameof(result));
        }
      }

      public ApiKeyModel? ApiKey { get; }
      public AuthenticateResult Result { get; } = null!;
    }
  }
}
