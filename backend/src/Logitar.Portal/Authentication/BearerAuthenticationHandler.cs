using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Constants;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Extensions;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Logitar.Portal.Authentication;

internal class BearerAuthenticationHandler : AuthenticationHandler<BearerAuthenticationOptions>
{
  private readonly ITokenService _tokenService;
  private readonly IUserQuerier _userQuerier;

  public BearerAuthenticationHandler(ITokenService tokenService, IUserQuerier userQuerier, IOptionsMonitor<BearerAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _tokenService = tokenService;
    _userQuerier = userQuerier;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (Context.Request.Headers.TryGetValue(Headers.Authorization, out StringValues authorization))
    {
      string? value = authorization.Single();
      if (!string.IsNullOrWhiteSpace(value))
      {
        string[] values = value.Split();
        if (values.Length != 2)
        {
          return AuthenticateResult.Fail($"The Authorization header value is not valid: '{value}'.");
        }
        else if (values[0] == Schemes.Bearer)
        {
          try
          {
            ValidateTokenPayload payload = new(values[1])
            {
              Type = "at+jwt"
            };
            ValidatedToken validatedToken = await _tokenService.ValidateAsync(payload);
            if (string.IsNullOrWhiteSpace(validatedToken.Subject))
            {
              return AuthenticateResult.Fail($"The '{nameof(validatedToken.Subject)}' claim is required.");
            }

            User? user = await _userQuerier.ReadAsync(realm: null, Guid.Parse(validatedToken.Subject.Trim()));
            if (user == null)
            {
              return AuthenticateResult.Fail($"The user 'Id={validatedToken.Subject}' could not be found.");
            }

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
