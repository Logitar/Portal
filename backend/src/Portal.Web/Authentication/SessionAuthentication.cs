using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Portal.Core.Claims;
using Portal.Core.Sessions;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Portal.Web.Authentication
{
  internal class SessionAuthenticationOptions : AuthenticationSchemeOptions
  {
  }

  internal class SessionAuthenticationHandler : AuthenticationHandler<SessionAuthenticationOptions>
  {
    private readonly ISessionQuerier _sessionQuerier;

    public SessionAuthenticationHandler(
      ISessionQuerier sessionQuerier,
      IOptionsMonitor<SessionAuthenticationOptions> options,
      ILoggerFactory logger,
      UrlEncoder encoder,
      ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
      _sessionQuerier = sessionQuerier;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      Guid? sessionId = Context.GetSessionId();
      if (sessionId.HasValue)
      {
        Session? session = await _sessionQuerier.GetAsync(sessionId.Value, readOnly: true);
        if (session == null)
        {
          Context.Session.Clear();

          return AuthenticateResult.Fail($"The session 'Id={sessionId}' could not be found.");
        }
        else if (!session.IsActive)
        {
          Context.Session.Clear();

          return AuthenticateResult.Fail($"The session 'Id={sessionId}' has ended.");
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

      return AuthenticateResult.NoResult();
    }
  }
}
