using Bogus;
using Logitar.EventSourcing;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class CacheServiceTests
{
  private readonly Faker _faker = new();

  private readonly ICacheService _cacheService;

  private readonly ConfigurationAggregate _configuration;
  private readonly UserAggregate _user;
  private readonly SessionAggregate _session;
  private readonly Actor _actor;

  public CacheServiceTests() : base()
  {
    IServiceProvider serviceProvider = new ServiceCollection()
      .AddMemoryCache()
      .AddSingleton<ICacheService, CacheService>()
      .BuildServiceProvider();

    _cacheService = serviceProvider.GetRequiredService<ICacheService>();

    Locale locale = new(_faker.Locale);
    _configuration = new(locale);

    _user = new(_configuration.UniqueNameSettings, _faker.Person.UserName)
    {
      Email = new EmailAddress(_faker.Person.Email),
      FirstName = _faker.Person.FirstName,
      LastName = _faker.Person.LastName,
      Birthdate = _faker.Person.DateOfBirth,
      Gender = new Gender(_faker.Person.Gender.ToString()),
      Locale = locale,
      Picture = new Uri(_faker.Person.Avatar),
      Website = new Uri($"https://www.{_faker.Person.Website}/")
    };

    _session = _user.SignIn(_configuration.UserSettings);

    _actor = new()
    {
      Id = _user.Id.ToGuid(),
      Type = ActorType.User,
      DisplayName = _user.FullName ?? _user.UniqueName,
      EmailAddress = _user.Email.Address,
      PictureUrl = _user.Picture.ToString()
    };
  }

  protected ActorId ActorId => new(_actor.Id);

  [Fact(DisplayName = "It should be able to remove a cached actor.")]
  public void It_should_be_able_to_remove_a_cached_actor()
  {
    _cacheService.SetActor(ActorId, _actor);
    Assert.NotNull(_cacheService.GetActor(ActorId));

    _cacheService.RemoveActor(ActorId);
    Assert.Null(_cacheService.GetActor(ActorId));
  }

  [Fact(DisplayName = "It should be able to remove a cached session.")]
  public void It_should_be_able_to_remove_a_cached_session()
  {
    Session session = new()
    {
      Id = _session.Id.ToGuid(),
      IsActive = true,
      User = new()
      {
        Id = _user.Id.ToGuid(),
        UniqueName = _user.UniqueName
      }
    };
    _cacheService.SetSession(session);
    Assert.NotNull(_cacheService.GetSession(session.Id));

    _cacheService.RemoveSession(_session);
    Assert.Null(_cacheService.GetSession(session.Id));
  }

  [Fact(DisplayName = "It should be able to remove a cached user.")]
  public void It_should_be_able_to_remove_a_cached_user()
  {
    User user = new()
    {
      Id = _user.Id.ToGuid(),
      UniqueName = _user.UniqueName
    };
    CachedUser cached = new(_user, user);
    _cacheService.SetUser(cached);
    Assert.NotNull(_cacheService.GetUser(_user.UniqueName));

    _cacheService.RemoveUser(_user);
    Assert.Null(_cacheService.GetUser(_user.UniqueName));
  }

  [Fact(DisplayName = "It should be able to retrieve a cached actor.")]
  public void It_should_be_able_to_retrieve_a_cached_actor()
  {
    _cacheService.SetActor(ActorId, _actor);

    Actor? cached = _cacheService.GetActor(ActorId);
    Assert.NotNull(cached);
    Assert.Equal(_actor, cached);
  }

  [Fact(DisplayName = "It should be able to retrieve a cached session.")]
  public void It_should_be_able_to_retrieve_a_cached_session()
  {
    Session session = new()
    {
      Id = _session.Id.ToGuid(),
      IsActive = true,
      User = new()
      {
        Id = _user.Id.ToGuid(),
        UniqueName = _user.UniqueName
      }
    };
    _cacheService.SetSession(session);

    Session? cached = _cacheService.GetSession(session.Id);
    Assert.NotNull(cached);
    Assert.Equal(session, cached);
  }

  [Fact(DisplayName = "It should be able to retrieve a cached user.")]
  public void It_should_be_able_to_retrieve_a_cached_user()
  {
    User user = new()
    {
      Id = _user.Id.ToGuid(),
      UniqueName = _user.UniqueName
    };
    CachedUser cached = new(_user, user);
    _cacheService.SetUser(cached);

    CachedUser? retrieved = _cacheService.GetUser(_user.UniqueName);
    Assert.NotNull(retrieved);
    Assert.Equal(cached, retrieved);
  }

  [Fact(DisplayName = "It should remove the cached sessions of a removed user.")]
  public void It_should_remove_the_cached_sessions_of_a_removed_user()
  {
    User user = new()
    {
      Id = _user.Id.ToGuid(),
      UniqueName = _user.UniqueName
    };
    CachedUser cached = new(_user, user);
    _cacheService.SetUser(cached);

    Session session = new()
    {
      Id = _session.Id.ToGuid(),
      IsActive = true,
      User = user
    };
    _cacheService.SetSession(session);

    _cacheService.RemoveUser(_user);
    Assert.Null(_cacheService.GetSession(session.Id));
  }
}
