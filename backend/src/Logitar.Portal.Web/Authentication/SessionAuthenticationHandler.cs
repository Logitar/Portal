using Logitar.Portal.Application.Claims;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Contracts.Sessions.Models;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Logitar.Portal.Web.Authentication
{
  internal class SessionAuthenticationHandler : AuthenticationHandler<SessionAuthenticationOptions>
  {
    private readonly ISessionQuerier _sessionQuerier;

    public SessionAuthenticationHandler(ISessionQuerier sessionQuerier,
      IOptionsMonitor<SessionAuthenticationOptions> options,
      ILoggerFactory logger,
      UrlEncoder encoder,
      ISystemClock clock) : base(options, logger, encoder, clock)
    {
      _sessionQuerier = sessionQuerier;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      string? sessionId = Context.GetSessionId();
      if (sessionId != null)
      {
        SessionModel? session = await _sessionQuerier.GetAsync(sessionId);
        AuthenticateResult? failure = null;
        if (session == null)
        {
          failure = AuthenticateResult.Fail($"The session 'Id={sessionId}' could not be found.");
        }
        else if (!session.IsActive)
        {
          failure = AuthenticateResult.Fail($"The session 'Id={session.Id}' has ended.");
        }
        else if (session.User == null)
        {
          failure = AuthenticateResult.Fail($"The User was null for session 'Id={session.Id}'.");
        }
        else if (session.User.IsDisabled)
        {
          failure = AuthenticateResult.Fail($"The User is disabled for session 'Id={session.Id}'.");
        }

        if (failure != null)
        {
          Context.Session.Clear();

          return failure;
        }

        if (!Context.SetSession(session))
        {
          throw new InvalidOperationException("The Session context item could not be set.");
        }
        if (!Context.SetUser(session!.User))
        {
          throw new InvalidOperationException("The User context item could not be set.");
        }

        ClaimsPrincipal principal = new(session.User!.GetClaimsIdentity(Constants.Schemes.Session));
        AuthenticationTicket ticket = new(principal, Constants.Schemes.Session);

        return AuthenticateResult.Success(ticket);
      }

      return AuthenticateResult.NoResult();
    }
  }
}
