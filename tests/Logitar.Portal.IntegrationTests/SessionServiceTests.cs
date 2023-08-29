﻿using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Realms;
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
      //Birthdate = Faker.Person.DateOfBirth,
      //Gender = new Gender(Faker.Person.Gender.ToString()),
      Locale = new Locale(Faker.Locale),
      //TimeZone = new TimeZoneEntry("America/Montreal"),
      //Picture = new Uri(Faker.Person.Avatar),
      //Website = _realm.Url
    };
    _user.Update(ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(new AggregateRoot[] { _realm, _user });
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

    RefreshToken refreshToken = RefreshToken.Decode(session.RefreshToken);
    SessionEntity? entity = await PortalContext.Sessions.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == refreshToken.Id.Value);
    Assert.NotNull(entity);

    Assert.NotNull(entity.Secret);
    Password secret = PasswordService.Decode(entity.Secret);
    Assert.True(secret.IsMatch(Convert.ToBase64String(refreshToken.Secret)));
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
    Assert.Equal(payload.UserId, exception.Id);
    Assert.Equal(nameof(payload.UserId), exception.PropertyName);
  }
}
