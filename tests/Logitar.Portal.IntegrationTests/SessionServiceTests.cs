using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class SessionServiceTests : IntegrationTests, IAsyncLifetime
{
  private readonly ISessionService _sessionService;

  private readonly RealmAggregate _realm;
  private readonly UserAggregate _user;

  private readonly byte[] _secret;
  private readonly SessionAggregate _session;

  public SessionServiceTests() : base()
  {
    _sessionService = ServiceProvider.GetRequiredService<ISessionService>();

    _realm = new("desjardins", requireUniqueEmail: true, requireConfirmedAccount: true)
    {
      DisplayName = "Desjardins",
      DefaultLocale = new Locale(Faker.Locale),
      Url = new Uri("https://www.desjardins.com/")
    };

    _user = new(_realm.UniqueNameSettings, Faker.Person.UserName, _realm.Id.Value)
    {
      Email = new EmailAddress(Faker.Person.Email, isVerified: true),
      FirstName = Faker.Person.FirstName,
      LastName = Faker.Person.LastName,
      Birthdate = Faker.Person.DateOfBirth,
      Gender = new Gender(Faker.Person.Gender.ToString()),
      Locale = new Locale(Faker.Locale),
      TimeZone = new TimeZoneEntry("America/Montreal"),
      Picture = new Uri(Faker.Person.Avatar),
      Website = _realm.Url
    };
    _user.Profile = new Uri($"{_realm.Url}profiles/{_user.Id.ToGuid()}");
    _user.SetPassword(PasswordService.Create(_realm.PasswordSettings, PasswordString));

    Password secret = PasswordService.Generate(_realm.PasswordSettings, SessionAggregate.SecretLength, out _secret);
    _session = new(_user, secret);
    _session.SetCustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}");
    _session.SetCustomAttribute("IpAddress", Faker.Internet.Ip());
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(new AggregateRoot[] { _realm, _user, _session });
  }

  [Fact(DisplayName = "CreateAsync: it should create a persistent session.")]
  public async Task CreateAsync_it_should_create_a_persistent_session()
  {
    Assert.NotNull(User);
    CreateSessionPayload payload = new()
    {
      UserId = User.Id.ToGuid(),
      IsPersistent = true
    };

    Session session = await _sessionService.CreateAsync(payload);
    Assert.True(session.IsPersistent);
    Assert.NotNull(session.RefreshToken);
    Assert.Equal(User.Id.ToGuid(), session.User.Id);

    await AssertRefreshTokenAsync(session.RefreshToken);
  }

  [Fact(DisplayName = "CreateAsync: it should create a session.")]
  public async Task CreateAsync_it_should_create_a_session()
  {
    CreateSessionPayload payload = new()
    {
      UserId = _user.Id.ToGuid(),
      IsPersistent = false,
      CustomAttributes = new CustomAttribute[]
      {
        new("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}"),
        new("IpAddress", Faker.Internet.Ip())
      }
    };

    Session session = await _sessionService.CreateAsync(payload);

    Assert.NotEqual(Guid.Empty, session.Id);
    Assert.Equal(Actor, session.CreatedBy);
    AssertIsNear(session.CreatedOn);
    Assert.Equal(Actor, session.UpdatedBy);
    AssertIsNear(session.UpdatedOn);
    Assert.True(session.Version >= 1);

    Assert.False(session.IsPersistent);
    Assert.Null(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(payload.CustomAttributes, session.CustomAttributes);

    Assert.Equal(_user.Id.ToGuid(), session.User.Id);
  }

  [Fact(DisplayName = "CreateAsync: it should throw AggregateNotFoundException when the user is not found.")]
  public async Task CreateAsync_it_should_throw_AggregateNotFoundException_when_the_user_is_not_found()
  {
    CreateSessionPayload payload = new()
    {
      UserId = Guid.Empty
    };
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<UserAggregate>>(async () => await _sessionService.CreateAsync(payload));
    Assert.Equal(payload.UserId.ToString(), exception.Id);
    Assert.Equal(nameof(payload.UserId), exception.PropertyName);
  }

  [Fact(DisplayName = "ReadAsync: it should read the session by ID.")]
  public async Task ReadAsync_it_should_read_the_session_by_Id()
  {
    Session? session = await _sessionService.ReadAsync(_session.Id.ToGuid());
    Assert.NotNull(session);
    Assert.Equal(_session.Id.ToGuid(), session.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should return null when the session is not found.")]
  public async Task ReadAsync_it_should_return_null_when_the_session_is_not_found()
  {
    Assert.Null(await _sessionService.ReadAsync(Guid.Empty));
  }

  [Fact(DisplayName = "RenewAsync: it should renew a Portal session.")]
  public async Task RenewAsync_it_should_renew_a_Portal_session()
  {
    Assert.NotNull(Configuration);
    Assert.NotNull(User);

    Password secret = PasswordService.Generate(Configuration.PasswordSettings, SessionAggregate.SecretLength, out byte[] secretBytes);
    SessionAggregate aggregate = new(User, secret);
    await AggregateRepository.SaveAsync(aggregate);

    RenewPayload payload = new()
    {
      RefreshToken = new RefreshToken(aggregate, secretBytes).Encode()
    };

    Session session = await _sessionService.RenewAsync(payload);

    Assert.Equal(aggregate.Id.ToGuid(), session.Id);
    Assert.True(session.IsPersistent);
    Assert.NotNull(session.RefreshToken);

    await AssertRefreshTokenAsync(session.RefreshToken);
  }

  [Fact(DisplayName = "RenewAsync: it should renew a session.")]
  public async Task RenewAsync_it_should_renew_a_session()
  {
    string ipAddress = Faker.Internet.Ip();
    string userAgent = Faker.Internet.UserAgent();
    RenewPayload payload = new()
    {
      RefreshToken = new RefreshToken(_session, _secret).Encode(),
      CustomAttributes = new CustomAttribute[]
      {
        new("IpAddress", ipAddress),
        new("UserAgent", userAgent)
      }
    };

    Session session = await _sessionService.RenewAsync(payload);

    Assert.Equal(_session.Id.ToGuid(), session.Id);
    Assert.Equal(Actor, session.UpdatedBy);
    AssertIsNear(session.UpdatedOn);
    Assert.True(session.Version > 1);

    Assert.True(session.IsPersistent);
    Assert.NotNull(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);

    Assert.Equal(_user.Id.ToGuid(), session.User.Id);

    Assert.Equal(3, session.CustomAttributes.Count());
    Assert.Contains(session.CustomAttributes, customAttribute => customAttribute.Key == "AdditionalInformation"
      && customAttribute.Value == _session.CustomAttributes["AdditionalInformation"]);
    Assert.Contains(session.CustomAttributes, customAttribute => customAttribute.Key == "IpAddress" && customAttribute.Value == ipAddress);
    Assert.Contains(session.CustomAttributes, customAttribute => customAttribute.Key == "UserAgent" && customAttribute.Value == userAgent);

    await AssertRefreshTokenAsync(session.RefreshToken);
  }

  [Fact(DisplayName = "RenewAsync: it should throw InvalidRefreshTokenException when the refresh token is not valid.")]
  public async Task RenewAsync_it_should_throw_InvalidRefreshTokenException_when_the_refresh_token_is_not_valid()
  {
    RenewPayload payload = new()
    {
      RefreshToken = Guid.NewGuid().ToString("N")
    };
    var exception = await Assert.ThrowsAsync<InvalidRefreshTokenException>(async () => await _sessionService.RenewAsync(payload));
    Assert.Equal(payload.RefreshToken, exception.RefreshToken);
  }

  [Fact(DisplayName = "RenewAsync: it should throw SessionNotFoundException when the session is not found.")]
  public async Task RenewAsync_it_should_throw_SessionNotFoundException_when_the_session_is_not_found()
  {
    SessionAggregate session = new(_user);
    RefreshToken refreshToken = new(session, Array.Empty<byte>());
    RenewPayload payload = new()
    {
      RefreshToken = refreshToken.Encode()
    };
    var exception = await Assert.ThrowsAsync<SessionNotFoundException>(async () => await _sessionService.RenewAsync(payload));
    Assert.Equal(refreshToken.Id.Value, exception.Id);
  }

  [Fact(DisplayName = "SearchAsync: it should return empty results when none are matching.")]
  public async Task SearchAsync_it_should_return_empty_results_when_none_are_matching()
  {
    SearchSessionsPayload payload = new()
    {
      IdIn = new[] { Guid.Empty }
    };

    SearchResults<Session> results = await _sessionService.SearchAsync(payload);

    Assert.Empty(results.Results);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    UserAggregate user = new(_realm.UniqueNameSettings, $"{_user.UniqueName}2", _realm.Id.Value);

    SessionAggregate idNotIn = new(_user);
    SessionAggregate otherUser = new(user);

    Password secret = PasswordService.Generate(_realm.PasswordSettings, SessionAggregate.SecretLength, out _);
    SessionAggregate persistent = new(_user, secret);

    SessionAggregate session1 = new(_user);
    SessionAggregate session2 = new(_user);
    SessionAggregate session3 = new(_user);
    SessionAggregate session4 = new(_user);

    SessionAggregate signedOut = new(_user);
    signedOut.SignOut();

    await AggregateRepository.SaveAsync(new AggregateRoot[] { user, idNotIn, otherUser, persistent, session1, session2, session3, session4, signedOut });

    SessionAggregate[] sessions = new[] { session1, session2, session3, session4 }
      .OrderByDescending(x => x.UpdatedOn).Skip(1).Take(2).ToArray();

    HashSet<Guid> ids = (await PortalContext.Sessions.AsNoTracking().ToArrayAsync())
      .Select(session => new AggregateId(session.AggregateId).ToGuid()).ToHashSet();
    ids.Remove(idNotIn.Id.ToGuid());

    SearchSessionsPayload payload = new()
    {
      Search = new TextSearch
      {
        Terms = new SearchTerm[]
        {
          new("test")
        }
      },
      IdIn = ids,
      Realm = _realm.UniqueSlug,
      UserId = _user.Id.ToGuid(),
      IsActive = true,
      IsPersistent = false,
      Sort = new SessionSortOption[]
      {
        new()
      },
      Skip = 1,
      Limit = 2
    };

    SearchResults<Session> results = await _sessionService.SearchAsync(payload);

    Assert.Equal(sessions.Length, results.Results.Count());
    Assert.Equal(4, results.Total);

    for (int i = 0; i < sessions.Length; i++)
    {
      Assert.Equal(sessions[i].Id.ToGuid(), results.Results.ElementAt(i).Id);
    }
  }

  [Fact(DisplayName = "SignInAsync: it should create a persistent session.")]
  public async Task SignInAsync_it_should_create_a_persistent_session()
  {
    Assert.NotNull(User);
    SignInPayload payload = new()
    {
      UniqueName = User.UniqueName,
      Password = PasswordString,
      IsPersistent = true
    };

    Session session = await _sessionService.SignInAsync(payload);
    Assert.True(session.IsPersistent);
    Assert.NotNull(session.RefreshToken);
    Assert.Equal(User.Id.ToGuid(), session.User.Id);

    await AssertRefreshTokenAsync(session.RefreshToken);
  }

  [Fact(DisplayName = "SignInAsync: it should create a session by email address.")]
  public async Task SignInAsync_it_should_create_a_session_by_email_address()
  {
    Assert.NotNull(_user.Email);
    SignInPayload payload = new()
    {
      Realm = $" {_realm.UniqueSlug.ToLower()} ",
      UniqueName = $" {_user.Email.Address.ToLower()} ",
      Password = PasswordString,
      IsPersistent = false,
      CustomAttributes = new CustomAttribute[]
  {
        new("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}"),
        new("IpAddress", Faker.Internet.Ip())
  }
    };

    Session session = await _sessionService.SignInAsync(payload);

    Assert.NotEqual(Guid.Empty, session.Id);
    Assert.Equal(Actor, session.CreatedBy);
    AssertIsNear(session.CreatedOn);
    Assert.Equal(Actor, session.UpdatedBy);
    AssertIsNear(session.UpdatedOn);
    Assert.True(session.Version >= 1);

    Assert.False(session.IsPersistent);
    Assert.Null(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(payload.CustomAttributes, session.CustomAttributes);

    Assert.Equal(_user.Id.ToGuid(), session.User.Id);
  }

  [Fact(DisplayName = "SignInAsync: it should create a session by unique name.")]
  public async Task SignInAsync_it_should_create_a_session_by_unique_name()
  {
    SignInPayload payload = new()
    {
      Realm = $" {_realm.UniqueSlug.ToLower()} ",
      UniqueName = $" {_user.UniqueName.ToLower()} ",
      Password = PasswordString,
      IsPersistent = false,
      CustomAttributes = new CustomAttribute[]
  {
        new("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}"),
        new("IpAddress", Faker.Internet.Ip())
  }
    };

    Session session = await _sessionService.SignInAsync(payload);

    Assert.NotEqual(Guid.Empty, session.Id);
    Assert.Equal(Actor, session.CreatedBy);
    AssertIsNear(session.CreatedOn);
    Assert.Equal(Actor, session.UpdatedBy);
    AssertIsNear(session.UpdatedOn);
    Assert.True(session.Version >= 1);

    Assert.False(session.IsPersistent);
    Assert.Null(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(payload.CustomAttributes, session.CustomAttributes);

    Assert.Equal(_user.Id.ToGuid(), session.User.Id);
  }

  [Fact(DisplayName = "SignInAsync: it should throw AggregateNotFoundException when the realm is not found.")]
  public async Task SignInAsync_it_should_throw_AggregateNotFoundException_when_the_realm_is_not_found()
  {
    SignInPayload payload = new()
    {
      Realm = $"{_realm.UniqueSlug}-2"
    };
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<RealmAggregate>>(async () => await _sessionService.SignInAsync(payload));
    Assert.Equal(payload.Realm, exception.Id);
    Assert.Equal(nameof(payload.Realm), exception.PropertyName);
  }

  [Fact(DisplayName = "SignInAsync: it should throw UserNotFoundException when the user is not found.")]
  public async Task SignInAsync_it_should_throw_UserNotFoundException_when_the_user_is_not_found()
  {
    SignInPayload payload = new()
    {
      Realm = _realm.Id.ToGuid().ToString(),
      UniqueName = $"{_user.UniqueName}2",
      Password = PasswordString
    };
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _sessionService.SignInAsync(payload));
    Assert.Equal(_realm.ToString(), exception.Realm);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
  }

  [Fact(DisplayName = "SignOutAsync: it should return null when the session is not found.")]
  public async Task SignOutAsync_it_should_return_null_when_the_session_is_not_found()
  {
    Assert.Null(await _sessionService.SignOutAsync(Guid.Empty));
  }

  [Fact(DisplayName = "SignOutAsync: it should sign out the session.")]
  public async Task SignOutAsync_it_should_sign_out_the_session()
  {
    Session? session = await _sessionService.SignOutAsync(_session.Id.ToGuid());
    Assert.NotNull(session);
    Assert.False(session.IsActive);
    Assert.Equal(Actor, session.SignedOutBy);
    Assert.True(session.SignedOutOn.HasValue);
    AssertIsNear(session.SignedOutOn.Value);

    Session? bis = await _sessionService.SignOutAsync(_session.Id.ToGuid());
    Assert.NotNull(bis);
    Assert.Equal(session.SignedOutBy, bis.SignedOutBy);
    Assert.Equal(session.SignedOutOn, bis.SignedOutOn);
    Assert.Equal(session.UpdatedBy, bis.UpdatedBy);
    Assert.Equal(session.UpdatedOn, bis.UpdatedOn);
    Assert.Equal(session.Version, bis.Version);
  }

  private async Task AssertRefreshTokenAsync(string value)
  {
    RefreshToken refreshToken = RefreshToken.Decode(value);
    SessionEntity? entity = await PortalContext.Sessions.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == refreshToken.Id.Value);
    Assert.NotNull(entity);

    Assert.NotNull(entity.Secret);
    Password secret = PasswordService.Decode(entity.Secret);
    Assert.True(secret.IsMatch(Convert.ToBase64String(refreshToken.Secret)));
  }
}
