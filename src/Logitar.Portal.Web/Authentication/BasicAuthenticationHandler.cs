﻿using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Core.Claims;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users;
using Logitar.Portal.Core.Users.Contact;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Logitar.Portal.Web.Authentication;

internal class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public BasicAuthenticationHandler(IRealmRepository realmRepository,
    IUserQuerier userQuerier,
    IUserRepository userRepository,
    IOptionsMonitor<BasicAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock) : base(options, logger, encoder, clock)
  {
    _realmRepository = realmRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (Context.Request.Headers.TryGetValue(Constants.Headers.Authorization, out StringValues authorization))
    {
      string? value = authorization.Single();
      if (!string.IsNullOrWhiteSpace(value))
      {
        string[] values = value.Split();
        if (values.Length != 2)
        {
          return AuthenticateResult.Fail($"The Authorization header value is not valid: '{value}'.");
        }
        else if (values[0] == Constants.Schemes.Basic)
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

          RealmAggregate? realm = await _realmRepository.LoadByUniqueNameAsync(Core.Constants.PortalRealm.UniqueName); // TODO(fpion): caching
          if (realm == null)
          {
            return AuthenticateResult.Fail($"The realm '{Core.Constants.PortalRealm.UniqueName}' could not be found.");
          }

          UserAggregate? user = await _userRepository.LoadAsync(realm, username);
          if (user == null && realm.RequireUniqueEmail)
          {
            ReadOnlyEmail email = new(username);
            IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(realm, email);
            if (users.Count() == 1)
            {
              user = users.Single();
            }
          }

          if (user == null)
          {
            return AuthenticateResult.Fail($"The Portal user could not be found: '{username}'.");
          }

          try
          {
            _ = user.SignIn(realm, password);
          }
          catch (Exception exception)
          {
            return AuthenticateResult.Fail(exception);
          }

          User userModel = await _userQuerier.GetAsync(user);

          Context.SetUser(userModel);

          ClaimsPrincipal principal = new(userModel.GetClaimsIdentity(Constants.Schemes.Basic));
          AuthenticationTicket ticket = new(principal, Constants.Schemes.Basic);

          return AuthenticateResult.Success(ticket);
        }
      }
    }

    return AuthenticateResult.NoResult();
  }
}
