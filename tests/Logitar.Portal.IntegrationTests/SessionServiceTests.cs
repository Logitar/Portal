using Logitar.EventSourcing;
using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Realms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class SessionServiceTests : IntegrationTests, IAsyncLifetime
{
  private readonly IPasswordService _passwordService;
  private readonly ISessionService _sessionService;

  private readonly RealmAggregate _realm;
  private readonly UserAggregate _user;
  private readonly SessionAggregate _session;

  private readonly byte[] _secretBytes;

  public SessionServiceTests()
  {
    _passwordService = ServiceProvider.GetRequiredService<IPasswordService>();
    _sessionService = ServiceProvider.GetRequiredService<ISessionService>();

    _realm = new("desjardins", requireUniqueEmail: true, requireConfirmedAccount: true, actorId: ActorId)
    {
      DisplayName = "Desjardins",
      DefaultLocale = new Locale(Faker.Locale),
      Url = new Uri("https://www.desjardins.com/")
    };
    _realm.Update(ActorId);

    _user = new(_realm.UniqueNameSettings, Faker.Person.UserName, _realm.Id.Value, ActorId)
    {
      Email = new EmailAddress(Faker.Person.Email, isVerified: true)
    };
    _user.SetPassword(_passwordService.Create(PasswordString));
    _user.Update(ActorId);

    Password secret = _passwordService.Generate(Application.Sessions.Constants.SecretLength, out _secretBytes);
    _session = _user.SignIn(_realm.UserSettings, secret, ActorId);
    _session.SetCustomAttribute("AdditionalInformation", "{}");
    _session.SetCustomAttribute("IpAddress", "::1");
    _session.Update(ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(new AggregateRoot[] { _realm, _user, _session });
  }

  [Fact(DisplayName = "CreateAsync: it should create a new session.")]
  public async Task CreateAsync_it_should_create_a_new_session()
  {
    Assert.NotNull(User);

    CreateSessionPayload payload = new()
    {
      UserId = User.Id.Value,
      CustomAttributes = new CustomAttribute[]
      {
        new("AdditionalInformation", "{}"),
        new("IpAddress", "::1")
      }
    };

    Session session = await _sessionService.CreateAsync(payload);

    Assert.Equal(Actor, session.CreatedBy);
    AssertIsNear(session.CreatedOn);
    Assert.Equal(Actor, session.UpdatedBy);
    AssertIsNear(session.UpdatedOn);
    Assert.True(session.Version >= 1);

    Assert.Null(session.RefreshToken);
    Assert.False(session.IsPersistent);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(payload.CustomAttributes, session.CustomAttributes);
    Assert.Equal(payload.UserId, session.User.Id);
    AssertIsNear(session.User.AuthenticatedOn);
  }

  [Fact(DisplayName = "CreateAsync: it should create a persistent session.")]
  public async Task CreateAsync_it_should_create_a_persistent_session()
  {
    CreateSessionPayload payload = new()
    {
      UserId = _user.Id.Value,
      IsPersistent = true
    };

    Session session = await _sessionService.CreateAsync(payload);

    Assert.NotNull(session.RefreshToken);
    Assert.True(session.IsPersistent);

    await CheckRefreshTokenAsync(session.RefreshToken);
  }

  [Fact(DisplayName = "CreateAsync: it should throw AggregateNotFoundException when the user could not be found.")]
  public async Task CreateAsync_it_should_throw_AggregateNotFoundException_when_the_user_could_not_be_found()
  {
    CreateSessionPayload payload = new()
    {
      UserId = Guid.Empty.ToString()
    };
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<UserAggregate>>(
      async () => await _sessionService.CreateAsync(payload)
    );
    Assert.Equal(payload.UserId, exception.Id);
    Assert.Equal(nameof(payload.UserId), exception.PropertyName);
  }

  [Fact(DisplayName = "CreateAsync: it should throw UserIsDisabledException when the user is disabled.")]
  public async Task CreateAsync_it_should_throw_UserIsDisabledException_when_the_user_is_disabled()
  {
    _user.Disable(ActorId);
    await AggregateRepository.SaveAsync(_user);

    CreateSessionPayload payload = new()
    {
      UserId = _user.Id.Value
    };
    var exception = await Assert.ThrowsAsync<UserIsDisabledException>(
      async () => await _sessionService.CreateAsync(payload)
    );
    Assert.Equal(_user.ToString(), exception.User);
  }

  [Fact(DisplayName = "CreateAsync: it should throw UserIsDisabledException when the user is not confirmed.")]
  public async Task CreateAsync_it_should_throw_UserIsDisabledException_when_the_user_is_not_confirmed()
  {
    _user.Email = null;
    _user.Update(ActorId);
    await AggregateRepository.SaveAsync(_user);

    CreateSessionPayload payload = new()
    {
      UserId = _user.Id.Value
    };
    var exception = await Assert.ThrowsAsync<UserIsNotConfirmedException>(
      async () => await _sessionService.CreateAsync(payload)
    );
    Assert.Equal(_user.ToString(), exception.User);
  }

  [Fact(DisplayName = "ReadAsync: it should return null when the session is not found.")]
  public async Task ReadAsync_it_should_return_null_when_the_session_is_not_found()
  {
    Assert.Null(await _sessionService.ReadAsync(Guid.Empty.ToString()));
  }

  [Fact(DisplayName = "ReadAsync: it should return the session found by ID.")]
  public async Task ReadAsync_it_should_return_the_session_found_by_id()
  {
    Session? session = await _sessionService.ReadAsync(_session.Id.Value);
    Assert.NotNull(session);
    Assert.Equal(_session.Id.Value, session.Id);
  }

  [Fact(DisplayName = "RenewAsync: it should renew the session.")]
  public async Task RenewAsync_it_should_renew_the_session()
  {
    RenewSessionPayload payload = new()
    {
      RefreshToken = new RefreshToken(_session, _secretBytes).Encode(),
      CustomAttributes = new CustomAttribute[]
      {
        new("AdditionalInformation", "{\"sec-ch-ua\":[0:\"\"Chromium\";v=\"116\", \"Not)A;Brand\";v=\"24\", \"Microsoft Edge\";v=\"116\"\"]}"),
        new("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.1938.54")
      }
    };

    Session session = await _sessionService.RenewAsync(payload);

    Assert.Equal(Actor, session.UpdatedBy);
    AssertIsNear(session.UpdatedOn);
    Assert.True(session.Version > 1);

    Assert.NotNull(session.RefreshToken);
    Assert.True(session.IsPersistent);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(_user.Id.Value, session.User.Id);

    Assert.Equal(3, session.CustomAttributes.Count());
    Assert.Contains(session.CustomAttributes, customAttribute => customAttribute.Key == "AdditionalInformation"
      && customAttribute.Value == "{\"sec-ch-ua\":[0:\"\"Chromium\";v=\"116\", \"Not)A;Brand\";v=\"24\", \"Microsoft Edge\";v=\"116\"\"]}");
    Assert.Contains(session.CustomAttributes, customAttribute => customAttribute.Key == "IpAddress"
      && customAttribute.Value == "::1");
    Assert.Contains(session.CustomAttributes, customAttribute => customAttribute.Key == "UserAgent"
      && customAttribute.Value == "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.1938.54");

    await CheckRefreshTokenAsync(session.RefreshToken);
  }

  [Fact(DisplayName = "RenewAsync: it should throw IncorrectSessionSecretException when the secret bytes are not valid.")]
  public async Task RenewAsync_it_should_throw_IncorrectSessionSecretException_when_the_secret_bytes_are_not_valid()
  {
    byte[] secretBytes = new byte[_secretBytes.Length];
    Array.Copy(_secretBytes, secretBytes, secretBytes.Length);
    secretBytes[0]++;

    RenewSessionPayload payload = new()
    {
      RefreshToken = new RefreshToken(_session, secretBytes).Encode()
    };
    var exception = await Assert.ThrowsAsync<IncorrectSessionSecretException>(
      async () => await _sessionService.RenewAsync(payload)
    );
    Assert.Equal(_session.ToString(), exception.Session);
    Assert.Equal(Convert.ToBase64String(secretBytes), exception.Secret);
  }

  [Fact(DisplayName = "RenewAsync: it should throw InvalidRefreshTokenException when the refresh token is not valid.")]
  public async Task RenewAsync_it_should_throw_InvalidRefreshTokenException_when_the_refresh_token_is_not_valid()
  {
    RenewSessionPayload payload = new()
    {
      RefreshToken = Guid.NewGuid().ToString()
    };
    var exception = await Assert.ThrowsAsync<InvalidRefreshTokenException>(
      async () => await _sessionService.RenewAsync(payload)
    );
    Assert.Equal(payload.RefreshToken, exception.RefreshToken);
  }

  [Fact(DisplayName = "RenewAsync: it should throw SessionIsNotActiveException when the session is not active.")]
  public async Task RenewAsync_it_should_throw_SessionIsNotActiveException_when_the_session_is_not_active()
  {
    _session.SignOut(ActorId);
    await AggregateRepository.SaveAsync(_session);

    RenewSessionPayload payload = new()
    {
      RefreshToken = new RefreshToken(_session, _secretBytes).Encode()
    };
    var exception = await Assert.ThrowsAsync<SessionIsNotActiveException>(
      async () => await _sessionService.RenewAsync(payload)
    );
    Assert.Equal(_session.ToString(), exception.Session);
  }

  [Fact(DisplayName = "RenewAsync: it should throw SessionNotFoundException when the session could not be found.")]
  public async Task RenewAsync_it_should_throw_SessionNotFoundException_when_the_session_could_not_be_found()
  {
    _session.Delete(ActorId);
    await AggregateRepository.SaveAsync(_session);

    RenewSessionPayload payload = new()
    {
      RefreshToken = new RefreshToken(_session, _secretBytes).Encode()
    };
    var exception = await Assert.ThrowsAsync<SessionNotFoundException>(
      async () => await _sessionService.RenewAsync(payload)
    );
    Assert.Equal(_session.Id.Value, exception.Id);
  }

  [Fact(DisplayName = "SearchAsync: it should return no result when there are no match.")]
  public async Task SearchAsync_it_should_return_no_result_when_there_are_no_match()
  {
    Assert.NotNull(User);
    SearchSessionsPayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      UserId = User.Id.Value
    };

    SearchResults<Session> results = await _sessionService.SearchAsync(payload);

    Assert.Empty(results.Results);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results by ID.")]
  public async Task SearchAsync_it_should_return_the_correct_results_by_ID()
  {
    SessionAggregate session1 = _user.SignIn(_realm.UserSettings, password: null, secret: null); // TODO(fpion): simplify method
    SessionAggregate session2 = _user.SignIn(_realm.UserSettings, password: null, secret: null); // TODO(fpion): simplify method
    await AggregateRepository.SaveAsync(new[] { session1, session2 });

    SearchSessionsPayload payload = new()
    {
      Realm = $" {_realm.UniqueSlug} ",
      Id = new TextSearch
      {
        Operator = QueryOperator.Or,
        Terms = new SearchTerm[]
        {
          new(_session.Id.Value),
          new(session1.Id.Value),
          new(Guid.Empty.ToString())
        }
      }
    };

    SearchResults<Session> results = await _sessionService.SearchAsync(payload);

    Assert.Equal(2, results.Results.Count());
    Assert.Equal(2, results.Total);

    Assert.Contains(results.Results, session => session.Id == _session.Id.Value);
    Assert.Contains(results.Results, session => session.Id == session1.Id.Value);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    Assert.NotNull(Configuration);
    Assert.NotNull(User);
    SessionAggregate session = User.SignIn(Configuration.UserSettings, password: null, secret: null); // TODO(fpion): simplify method

    UserAggregate user = new(_realm.UniqueNameSettings, $"{_user.UniqueName}2", _realm.Id.Value, ActorId)
    {
      Email = new EmailAddress(Faker.Person.Email, isVerified: true)
    };
    SessionAggregate other = user.SignIn(_realm.UserSettings, password: null, secret: null); // TODO(fpion): simplify method

    SessionAggregate signedOut = _user.SignIn(_realm.UserSettings, password: null, secret: null); // TODO(fpion): simplify method
    signedOut.SignOut();

    Password secret = _passwordService.Generate(Application.Sessions.Constants.SecretLength, out _);
    SessionAggregate persistent = _user.SignIn(_realm.UserSettings, secret);

    SessionAggregate session1 = _user.SignIn(_realm.UserSettings, password: null, secret: null); // TODO(fpion): simplify method
    SessionAggregate session2 = _user.SignIn(_realm.UserSettings, password: null, secret: null); // TODO(fpion): simplify method
    SessionAggregate session3 = _user.SignIn(_realm.UserSettings, password: null, secret: null); // TODO(fpion): simplify method
    SessionAggregate session4 = _user.SignIn(_realm.UserSettings, password: null, secret: null); // TODO(fpion): simplify method

    await AggregateRepository.SaveAsync(new AggregateRoot[] { user, session, other, signedOut, persistent, session1, session2, session3, session4 });

    SessionAggregate[] sessions = new[] { session1, session2, session3, session4 }
      .OrderByDescending(s => s.UpdatedOn)
      .Skip(1).Take(2)
      .ToArray();

    SearchSessionsPayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      UserId = _user.Id.Value,
      Search = new TextSearch
      {
        Terms = new SearchTerm[]
        {
          new("test")
        }
      },
      IsActive = true,
      IsPersistent = false,
      Sort = new SessionSortOption[]
      {
        new SessionSortOption()
      },
      Skip = 1,
      Limit = 2
    };

    SearchResults<Session> results = await _sessionService.SearchAsync(payload);

    Assert.Equal(sessions.Length, results.Results.Count());
    Assert.Equal(4, results.Total);

    for (int i = 0; i < sessions.Length; i++)
    {
      Assert.Equal(sessions[i].Id.Value, results.Results.ElementAt(i).Id);
    }
  }

  [Fact(DisplayName = "SignInAsync: it should create a persistent session.")]
  public async Task SignInAsync_it_should_create_a_persistent_session()
  {
    SignInPayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      UniqueName = _user.UniqueName,
      Password = PasswordString,
      IsPersistent = true
    };

    Session session = await _sessionService.SignInAsync(payload);

    Assert.NotNull(session.RefreshToken);
    Assert.True(session.IsPersistent);

    await CheckRefreshTokenAsync(session.RefreshToken);
  }

  [Fact(DisplayName = "SignInAsync: it should sign-in the user.")]
  public async Task SignInAsync_it_should_sign_in_the_user()
  {
    Assert.NotNull(User);

    SignInPayload payload = new()
    {
      UniqueName = User.UniqueName,
      Password = PasswordString,
      CustomAttributes = new CustomAttribute[]
      {
        new("AdditionalInformation", "{}"),
        new("IpAddress", "::1")
      }
    };

    Session session = await _sessionService.SignInAsync(payload);

    Assert.Equal(Actor, session.CreatedBy);
    AssertIsNear(session.CreatedOn);
    Assert.Equal(Actor, session.UpdatedBy);
    AssertIsNear(session.UpdatedOn);
    Assert.True(session.Version >= 1);

    Assert.Null(session.RefreshToken);
    Assert.False(session.IsPersistent);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(payload.CustomAttributes, session.CustomAttributes);
    Assert.Equal(User.Id.Value, session.User.Id);
    AssertIsNear(session.User.AuthenticatedOn);
  }

  [Fact(DisplayName = "SignInAsync: it should throw AggregateNotFoundException when the realm could not be found.")]
  public async Task SignInAsync_it_should_throw_AggregateNotFoundException_when_the_realm_could_not_be_found()
  {
    SignInPayload payload = new()
    {
      Realm = Guid.Empty.ToString()
    };
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<RealmAggregate>>(
      async () => await _sessionService.SignInAsync(payload)
    );
    Assert.Equal(payload.Realm, exception.Id);
    Assert.Equal(nameof(payload.Realm), exception.PropertyName);
  }

  [Fact(DisplayName = "SignInAsync: it should throw IncorrectUserPasswordException when the user could not be found.")]
  public async Task SignInAsync_it_should_throw_IncorrectUserPasswordException_when_the_user_could_not_be_found()
  {
    SignInPayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      UniqueName = _user.UniqueName,
      Password = PasswordString[1..]
    };
    var exception = await Assert.ThrowsAsync<IncorrectUserPasswordException>(
      async () => await _sessionService.SignInAsync(payload)
    );
    Assert.Equal(_user.ToString(), exception.User);
    Assert.Equal(payload.Password, exception.Password);
  }

  [Fact(DisplayName = "SignInAsync: it should throw UserIsDisabledException when the user is disabled.")]
  public async Task SignInAsync_it_should_throw_UserIsDisabledException_when_the_user_is_disabled()
  {
    _user.Disable(ActorId);
    await AggregateRepository.SaveAsync(_user);

    SignInPayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      UniqueName = _user.UniqueName,
      Password = PasswordString
    };
    var exception = await Assert.ThrowsAsync<UserIsDisabledException>(
      async () => await _sessionService.SignInAsync(payload)
    );
    Assert.Equal(_user.ToString(), exception.User);
  }

  [Fact(DisplayName = "SignInAsync: it should throw UserIsNotConfirmedException when the user is not confirmed.")]
  public async Task SignInAsync_it_should_throw_UserIsNotConfirmedException_when_the_user_is_not_confirmed()
  {
    _user.Email = null;
    _user.Update(ActorId);
    await AggregateRepository.SaveAsync(_user);

    SignInPayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      UniqueName = _user.UniqueName,
      Password = PasswordString
    };
    var exception = await Assert.ThrowsAsync<UserIsNotConfirmedException>(
      async () => await _sessionService.SignInAsync(payload)
    );
    Assert.Equal(_user.ToString(), exception.User);
  }

  [Fact(DisplayName = "SignInAsync: it should throw UserNotFoundException when the user could not be found.")]
  public async Task SignInAsync_it_should_throw_UserNotFoundException_when_the_user_could_not_be_found()
  {
    SignInPayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      UniqueName = $" {_user.UniqueName}2 "
    };
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(
      async () => await _sessionService.SignInAsync(payload)
    );
    Assert.Equal(_realm.ToString(), exception.Realm);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
  }

  [Fact(DisplayName = "SignOutAsync: it should return null when the session could not be found.")]
  public async Task SignOutAsync_it_should_return_null_when_the_session_could_not_be_found()
  {
    Assert.Null(await _sessionService.SignOutAsync(Guid.Empty.ToString()));
  }

  [Fact(DisplayName = "SignOutAsync: it should return the signed out session.")]
  public async Task SignOutAsync_it_should_return_the_signed_out_session()
  {
    Session? session = await _sessionService.SignOutAsync(_session.Id.Value);
    Assert.NotNull(session);
    Assert.Equal(Actor, session.UpdatedBy);
    AssertIsNear(session.UpdatedOn);
    Assert.Null(session.RefreshToken);
    Assert.Equal(_session.Id.Value, session.Id);
    Assert.False(session.IsActive);
    Assert.Equal(Actor, session.SignedOutBy);
    AssertIsNear(session.SignedOutOn);

    Session? same = await _sessionService.SignOutAsync(_session.Id.Value);
    Assert.NotNull(same);
    Assert.Equal(session.Version, same.Version);
  }

  private async Task CheckRefreshTokenAsync(string value)
  {
    RefreshToken refreshToken = RefreshToken.Decode(value);

    SessionEntity? session = await IdentityContext.Sessions.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == refreshToken.Id.Value);
    Assert.NotNull(session);

    Assert.NotNull(session.Secret);
    Password secret = _passwordService.Decode(session.Secret);

    Assert.True(secret.IsMatch(Convert.ToBase64String(refreshToken.Secret)));
  }
}
