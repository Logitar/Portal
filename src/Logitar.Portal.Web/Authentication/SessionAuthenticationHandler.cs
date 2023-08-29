using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Web.Authentication;

internal class SessionAuthenticationHandler : AuthenticationHandler<SessionAuthenticationOptions>
{
  private readonly ISessionQuerier _sessionQuerier;

  public SessionAuthenticationHandler(ISessionQuerier sessionQuerier, IOptionsMonitor<SessionAuthenticationOptions> options,
    ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
  {
    _sessionQuerier = sessionQuerier;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    Guid? sessionId = Context.GetSessionId();
    if (sessionId.HasValue)
    {
      Session? session = /*_cacheService.GetSession(sessionId.Value) ?*/ await _sessionQuerier.ReadAsync(sessionId.Value); // TODO(fpion): implement
      AuthenticateResult? failure = null;
      if (session == null)
      {
        failure = AuthenticateResult.Fail($"The session 'Id={sessionId}' could not be found.");
      }
      else if (!session.IsActive)
      {
        failure = AuthenticateResult.Fail($"The session 'Id={session.Id}' has ended.");
      }
      else if (session.User.IsDisabled)
      {
        failure = AuthenticateResult.Fail($"The User is disabled for session 'Id={session.Id}'.");
      }

      if (failure != null)
      {
        Context.SignOut();

        return failure;
      }

      //_cacheService.SetSession(session!); // TODO(fpion): implement

      Context.SetUser(session!.User);

      ClaimsPrincipal principal = new(session.User.CreateClaimsIdentity(Scheme.Name));
      AuthenticationTicket ticket = new(principal, Scheme.Name);

      return AuthenticateResult.Success(ticket);
    }

    return AuthenticateResult.NoResult();
  }
}
