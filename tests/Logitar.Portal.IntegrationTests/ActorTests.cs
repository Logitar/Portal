using Bogus;
using Logitar.EventSourcing;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class ActorTests : IntegrationTests, IAsyncLifetime
{
  private readonly IApiKeyService _apiKeyService;
  private readonly ICacheService _cacheService;
  private readonly IUserService _userService;

  public ActorTests() : base()
  {
    _apiKeyService = ServiceProvider.GetRequiredService<IApiKeyService>();
    _cacheService = ServiceProvider.GetRequiredService<ICacheService>();
    _userService = ServiceProvider.GetRequiredService<IUserService>();
  }

  [Fact(DisplayName = "Creating an API key should create an actor and remove it from cache.")]
  public async Task Creating_an_Api_key_should_create_an_actor_and_remove_it_from_cache()
  {
    CreateApiKeyPayload payload = new()
    {
      DisplayName = "Default"
    };

    ApiKey? apiKey = await _apiKeyService.CreateAsync(payload);
    Assert.NotNull(apiKey);

    ActorEntity? actor = await PortalContext.Actors.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == apiKey.Id);
    Assert.NotNull(actor);
    Assert.Equal(apiKey.Id, actor.Id);
    Assert.Equal(ActorType.ApiKey, actor.Type);
    Assert.False(actor.IsDeleted);
    Assert.Equal(apiKey.DisplayName, actor.DisplayName);
    Assert.Null(actor.EmailAddress);
    Assert.Null(actor.PictureUrl);

    ActorId actorId = new ActorId(apiKey.Id);
    Assert.Null(_cacheService.GetActor(actorId));
  }

  [Fact(DisplayName = "Creating an user should create an actor and remove it from cache.")]
  public async Task Creating_an_user_should_create_an_actor_and_remove_it_from_cache()
  {
    Faker faker = new();
    CreateUserPayload payload = new()
    {
      UniqueName = faker.Person.UserName,
      Email = new EmailPayload
      {
        Address = faker.Person.Email
      },
      FirstName = faker.Person.FirstName,
      LastName = faker.Person.LastName,
      Birthdate = faker.Person.DateOfBirth,
      Gender = faker.Person.Gender.ToString(),
      Locale = faker.Locale,
      Picture = faker.Person.Avatar
    };

    User? user = await _userService.CreateAsync(payload);
    Assert.NotNull(user);

    ActorEntity? actor = await PortalContext.Actors.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == user.Id);
    Assert.NotNull(actor);
    Assert.Equal(user.Id, actor.Id);
    Assert.Equal(ActorType.User, actor.Type);
    Assert.False(actor.IsDeleted);
    Assert.Equal(user.FullName, actor.DisplayName);
    Assert.Equal(user.Email?.Address, actor.EmailAddress);
    Assert.Equal(user.Picture, actor.PictureUrl);

    ActorId actorId = new(user.Id);
    Assert.Null(_cacheService.GetActor(actorId));
  }

  [Fact(DisplayName = "Deleting an API key should mark its actor as deleted and remove it from cache.")]
  public async Task Deleting_an_Api_key_should_mark_its_actor_as_deleted_and_remove_it_from_cache()
  {
    Assert.NotNull(Configuration);

    Password secret = PasswordService.Generate(Configuration.PasswordSettings, ApiKeyAggregate.SecretLength, out _);
    ApiKeyAggregate aggregate = new("Default", secret);
    await AggregateRepository.SaveAsync(aggregate);

    ActorId actorId = new(aggregate.Id.ToGuid());
    Actor cached = new()
    {
      Id = actorId.ToGuid(),
      Type = ActorType.ApiKey,
      IsDeleted = false,
      DisplayName = aggregate.DisplayName
    };
    _cacheService.SetActor(actorId, cached);

    ApiKey? apiKey = await _apiKeyService.DeleteAsync(aggregate.Id.ToGuid());
    Assert.NotNull(apiKey);

    ActorEntity? actor = await PortalContext.Actors.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == apiKey.Id);
    Assert.NotNull(actor);
    Assert.Equal(apiKey.Id, actor.Id);
    Assert.True(actor.IsDeleted);

    Assert.Null(_cacheService.GetActor(actorId));
  }

  [Fact(DisplayName = "Deleting an user should mark its actor as deleted and remove it from cache.")]
  public async Task Deleting_an_user_should_mark_its_actor_as_deleted_and_remove_it_from_cache()
  {
    Assert.NotNull(Configuration);

    Faker faker = new();
    UserAggregate aggregate = new(Configuration.UniqueNameSettings, faker.Person.UserName)
    {
      Email = new EmailAddress(faker.Person.Email),
      FirstName = faker.Person.FirstName,
      LastName = faker.Person.LastName,
      Birthdate = faker.Person.DateOfBirth,
      Gender = new Gender(faker.Person.Gender.ToString()),
      Locale = new ReadOnlyLocale(faker.Locale),
      Picture = new Uri(faker.Person.Avatar)
    };
    await AggregateRepository.SaveAsync(aggregate);

    ActorId actorId = new(aggregate.Id.ToGuid());
    Actor cached = new()
    {
      Id = actorId.ToGuid(),
      Type = ActorType.User,
      IsDeleted = false,
      DisplayName = aggregate.FullName ?? aggregate.UniqueName,
      EmailAddress = aggregate.Email.Address,
      PictureUrl = aggregate.Picture.ToString()
    };
    _cacheService.SetActor(actorId, cached);

    User? user = await _userService.DeleteAsync(aggregate.Id.ToGuid());
    Assert.NotNull(user);

    ActorEntity? actor = await PortalContext.Actors.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == user.Id);
    Assert.NotNull(actor);
    Assert.Equal(user.Id, actor.Id);
    Assert.True(actor.IsDeleted);

    Assert.Null(_cacheService.GetActor(actorId));
  }

  [Fact(DisplayName = "Updating an API key should update its actor and remove it from cache.")]
  public async Task Updating_an_Api_key_should_update_its_actor_and_remove_it_from_cache()
  {
    Assert.NotNull(Configuration);

    Password secret = PasswordService.Generate(Configuration.PasswordSettings, ApiKeyAggregate.SecretLength, out _);
    ApiKeyAggregate aggregate = new("Default", secret);
    await AggregateRepository.SaveAsync(aggregate);

    ActorId actorId = new(aggregate.Id.ToGuid());
    Actor cached = new()
    {
      Id = actorId.ToGuid(),
      Type = ActorType.ApiKey,
      IsDeleted = false,
      DisplayName = aggregate.DisplayName
    };
    _cacheService.SetActor(actorId, cached);

    UpdateApiKeyPayload payload = new()
    {
      DisplayName = "Par défaut"
    };
    ApiKey? apiKey = await _apiKeyService.UpdateAsync(aggregate.Id.ToGuid(), payload);
    Assert.NotNull(apiKey);

    ActorEntity? actor = await PortalContext.Actors.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == apiKey.Id);
    Assert.NotNull(actor);
    Assert.Equal(apiKey.Id, actor.Id);
    Assert.Equal(ActorType.ApiKey, actor.Type);
    Assert.False(actor.IsDeleted);
    Assert.Equal(apiKey.DisplayName, actor.DisplayName);
    Assert.Null(actor.EmailAddress);
    Assert.Null(actor.PictureUrl);

    Assert.Null(_cacheService.GetActor(actorId));
  }

  [Fact(DisplayName = "Updating an user should update its actor and remove it from cache.")]
  public async Task Updating_an_user_should_update_its_actor_and_remove_it_from_cache()
  {
    Assert.NotNull(Configuration);

    Faker faker = new();
    UserAggregate aggregate = new(Configuration.UniqueNameSettings, faker.Person.UserName)
    {
      FirstName = faker.Person.FirstName,
      LastName = faker.Person.LastName,
      Birthdate = faker.Person.DateOfBirth,
      Gender = new Gender(faker.Person.Gender.ToString()),
      Locale = new ReadOnlyLocale(faker.Locale),
      Picture = new Uri(faker.Person.Avatar)
    };
    await AggregateRepository.SaveAsync(aggregate);

    ActorId actorId = new(aggregate.Id.ToGuid());
    Actor cached = new()
    {
      Id = actorId.ToGuid(),
      Type = ActorType.User,
      IsDeleted = false,
      DisplayName = aggregate.FullName ?? aggregate.UniqueName,
      EmailAddress = aggregate.Email?.Address,
      PictureUrl = aggregate.Picture.ToString()
    };
    _cacheService.SetActor(actorId, cached);

    UpdateUserPayload payload = new()
    {
      Email = new Modification<EmailPayload>(new EmailPayload
      {
        Address = faker.Person.Email
      }),
      FirstName = new Modification<string>(null),
      LastName = new Modification<string>(null),
      Picture = new Modification<string>(null)
    };
    User? user = await _userService.UpdateAsync(aggregate.Id.ToGuid(), payload);
    Assert.NotNull(user);

    ActorEntity? actor = await PortalContext.Actors.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == user.Id);
    Assert.NotNull(actor);
    Assert.Equal(user.Id, actor.Id);
    Assert.Equal(ActorType.User, actor.Type);
    Assert.False(actor.IsDeleted);
    Assert.Equal(user.UniqueName, actor.DisplayName);
    Assert.Equal(faker.Person.Email, actor.EmailAddress);
    Assert.Null(actor.PictureUrl);

    Assert.Null(_cacheService.GetActor(actorId));
  }
}
