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

  public SessionServiceTests()
  {
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
      Email = new EmailAddress(Faker.Person.Email, isVerified: true),
      FirstName = Faker.Person.FirstName,
      LastName = Faker.Person.LastName,
      Birthdate = Faker.Person.DateOfBirth,
      //Gender = new Gender(Faker.Person.Gender.ToString()), // TODO(fpion): implement
      Locale = new Locale(Faker.Locale),
      //TimeZone = new TimeZoneEntry("America/Montreal"), // TODO(fpion): implement
      //Picture = new Uri(Faker.Person.Avatar), // TODO(fpion): implement
      //Website = _realm.Url // TODO(fpion): implement
    };
    //_user.Profile = new Uri($"{_realm.Url}profiles/{_user.Id.ToGuid()}"); // TODO(fpion): implement
    _user.SetPassword(PasswordService.Create(_realm.PasswordSettings, PasswordString));
    _user.Update(ActorId);

    Password secret = PasswordService.Generate(_realm.PasswordSettings, SessionAggregate.SecretLength, out _secret);
    _session = new(_user, secret, ActorId);
    _session.SetCustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}");
    _session.SetCustomAttribute("IpAddress", Faker.Internet.Ip());
    _session.Update(ActorId);
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

  [Fact(DisplayName = "RenewAsync: it should renew a Portal session.")]
  public async Task RenewAsync_it_should_renew_a_Portal_session()
  {
    Assert.NotNull(Configuration);
    Assert.NotNull(User);

    Password secret = PasswordService.Generate(Configuration.PasswordSettings, SessionAggregate.SecretLength, out byte[] secretBytes);
    SessionAggregate aggregate = new(User, secret, ActorId);
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

  [Fact(DisplayName = "SignInAsync: it should create a session.")]
  public async Task SignInAsync_it_should_create_a_session()
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
