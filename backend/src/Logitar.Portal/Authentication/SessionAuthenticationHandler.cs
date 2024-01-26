using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Authentication;

internal class SessionAuthenticationHandler : AuthenticationHandler<SessionAuthenticationOptions>
{
  private readonly ICacheService _cacheService;
  private readonly ISessionQuerier _sessionQuerier;

  public SessionAuthenticationHandler(ICacheService cacheService, ISessionQuerier sessionQuerier, IOptionsMonitor<SessionAuthenticationOptions> options,
    ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
  {
    _cacheService = cacheService;
    _sessionQuerier = sessionQuerier;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    string? sessionId = Context.GetSessionId();
    if (sessionId != null)
    {
      Session? session = /*_cacheService.GetSession(sessionId.Value) ??*/ await _sessionQuerier.ReadAsync(sessionId); // TODO(fpion): caching
      if (session == null)
      {
        return Fail($"The session 'Id={sessionId}' could not be found.");
      }
      else if (!session.IsActive)
      {
        return Fail($"The session 'Id={session.Id}' has ended.");
      }
      else if (session.User.IsDisabled)
      {
        return Fail($"The User is disabled for session 'Id={session.Id}'.");
      }

      //_cacheService.SetSession(session);

      Context.SetSession(session);
      Context.SetUser(session.User);

      ClaimsPrincipal principal = new(session.CreateClaimsIdentity(Scheme.Name));
      AuthenticationTicket ticket = new(principal, Scheme.Name);

      return AuthenticateResult.Success(ticket);
    }

    return AuthenticateResult.NoResult();
  }

  private AuthenticateResult Fail(string reason)
  {
    Context.SignOut();

    return AuthenticateResult.Fail(reason);
  }
}
