using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Constants;
using Logitar.Portal.Contracts.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Logitar.Portal.Authentication;

internal class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
  private readonly ICacheService _cacheService;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public BasicAuthenticationHandler(ICacheService cacheService, IUserQuerier userQuerier,
    IUserRepository userRepository, IOptionsMonitor<BasicAuthenticationOptions> options,
    ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
      : base(options, logger, encoder, clock)
  {
    _cacheService = cacheService;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
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
        else if (values[0] == Schemes.Basic)
        {
          byte[] bytes = Convert.FromBase64String(values[1]);
          string credentials = Encoding.UTF8.GetString(bytes);
          int index = credentials.IndexOf(':');
          if (index <= 0)
          {
            return AuthenticateResult.Fail($"The Basic credentials are not valid: '{credentials}'.");
          }

          string username = credentials[..index];
          string password = credentials[(index + 1)..];

          CachedUser? cached = _cacheService.GetUser(username);
          UserAggregate? user = cached?.Aggregate ?? await _userRepository.LoadAsync(tenantId: null, username);
          if (user == null)
          {
            return AuthenticateResult.Fail($"The Portal user could not be found: '{username}'.");
          }

          try
          {
            _ = user.SignIn(userSettings, password);
          }
          catch (Exception exception)
          {
            return AuthenticateResult.Fail(exception);
          }

          User userModel = cached?.Model ?? await _userQuerier.ReadAsync(user);

          cached = new(user, userModel);
          _cacheService.SetUser(username, cached);

          Context.SetUser(userModel);

          ClaimsPrincipal principal = new(userModel.GetClaimsIdentity(Schemes.Basic));
          AuthenticationTicket ticket = new(principal, Schemes.Basic);

          return AuthenticateResult.Success(ticket);
        }
      }
    }

    return AuthenticateResult.NoResult();
  }
}
