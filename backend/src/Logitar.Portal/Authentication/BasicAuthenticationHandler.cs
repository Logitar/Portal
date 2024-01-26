using Logitar.Portal.Constants;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Logitar.Portal.Authentication;

internal class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
  private readonly IUserService _userService;

  public BasicAuthenticationHandler(IUserService userService, IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _userService = userService;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (Context.Request.Headers.TryGetValue(Headers.Authorization, out StringValues authorization))
    {
      string? value = authorization.Single();
      if (!string.IsNullOrWhiteSpace(value))
      {
        string[] parts = value.Split();
        if (parts.Length != 2)
        {
          return AuthenticateResult.Fail($"The Authorization header value is not valid: '{value}'.");
        }
        else if (parts[0] == Schemes.Basic)
        {
          byte[] bytes = Convert.FromBase64String(parts[1]);
          string credentials = Encoding.UTF8.GetString(bytes);
          int index = credentials.IndexOf(':');
          if (index <= 0)
          {
            return AuthenticateResult.Fail($"The Basic credentials are not valid: '{credentials}'.");
          }

          try
          {
            AuthenticateUserPayload payload = new(uniqueName: credentials[..index], password: credentials[(index + 1)..]);
            User user = await _userService.AuthenticateAsync(payload);
            Context.SetUser(user);

            ClaimsPrincipal principal = new(user.CreateClaimsIdentity(Scheme.Name));
            AuthenticationTicket ticket = new(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
          }
          catch (Exception exception)
          {
            return AuthenticateResult.Fail(exception);
          }
        }
      }
    }

    return AuthenticateResult.NoResult();
  }
}
