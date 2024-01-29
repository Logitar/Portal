using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Identity.Infrastructure.Passwords.Pbkdf2;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Configurations.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class InitializeConfigurationCommandTests : IntegrationTests
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IUserRepository _userRepository;

  public InitializeConfigurationCommandTests() : base(initializeConfiguration: false)
  {
    _cacheService = ServiceProvider.GetRequiredService<ICacheService>();
    _configurationRepository = ServiceProvider.GetRequiredService<IConfigurationRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [IdentityDb.Actors.Table, IdentityDb.CustomAttributes.Table, IdentityDb.Sessions.Table, IdentityDb.Users.Table];
    foreach (TableId table in tables)
    {
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should initialize the configuration.")]
  public async Task It_should_initialize_the_configuration()
  {
    CustomAttribute[] sessionCustomAttributes =
    [
      new("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}"),
      new("IpAddress", Faker.Internet.Ip())
    ];
    InitializeConfigurationPayload payload = new()
    {
      DefaultLocale = Faker.Locale,
      User = new UserPayload(Faker.Person.UserName, PasswordString)
      {
        Email = new EmailPayload(Faker.Person.Email, isVerified: false),
        FirstName = Faker.Person.FirstName,
        LastName = Faker.Person.LastName
      },
      Session = new SessionPayload(sessionCustomAttributes)
    };
    InitializeConfigurationCommand command = new(payload);
    Session session = await Mediator.Send(command);

    Assert.NotNull(_cacheService.Configuration);
    Assert.Equal(payload.DefaultLocale, _cacheService.Configuration.DefaultLocale);
    Assert.Equal(session.User.Id, _cacheService.Configuration.CreatedBy.Id);
    Assert.Equal(session.User.Id, _cacheService.Configuration.UpdatedBy.Id);

    Assert.False(session.IsPersistent);
    Assert.True(session.IsActive);
    foreach (CustomAttribute customAttribute in sessionCustomAttributes)
    {
      Assert.Contains(customAttribute, session.CustomAttributes);
    }
    Assert.Equal(session.User.Id, session.CreatedBy.Id);
    Assert.Equal(session.User.Id, session.UpdatedBy.Id);

    Assert.Equal(payload.User.UniqueName, session.User.UniqueName);
    Assert.True(session.User.HasPassword);
    Assert.False(session.User.IsDisabled);
    Assert.NotNull(session.User.Email);
    Assert.Equal(payload.User.Email.Address, session.User.Email.Address);
    Assert.False(session.User.IsConfirmed);
    Assert.Equal(payload.User.FirstName, session.User.FirstName);
    Assert.Equal(payload.User.LastName, session.User.LastName);
    Assert.Equal(Faker.Person.FullName, session.User.FullName);
    Assert.Equal(payload.DefaultLocale, session.User.Locale);
    Assert.NotNull(session.User.AuthenticatedOn);
    Assert.Null(session.User.Realm);
    Assert.Equal(session.User.Id, session.User.CreatedBy.Id);
    Assert.Equal(session.User.Id, session.User.UpdatedBy.Id);

    string aggregateId = new AggregateId(session.User.Id).Value;
    UserEntity? user = await IdentityContext.Users.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == aggregateId);
    Assert.NotNull(user);
    Assert.NotNull(user.PasswordHash);
    Pbkdf2Password password = Pbkdf2Password.Decode(user.PasswordHash);
    Assert.True(password.IsMatch(payload.User.Password));
  }

  [Fact(DisplayName = "It should remove the cached configuration when saving fails.")]
  public async Task It_should_remove_the_cached_configuration_when_saving_fails()
  {
    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    UserAggregate user = new(new UniqueNameUnit(uniqueNameSettings, Faker.Person.UserName), tenantId: null);
    await _userRepository.SaveAsync(user);

    InitializeConfigurationPayload payload = new()
    {
      DefaultLocale = Faker.Locale,
      User = new UserPayload(Faker.Person.UserName, PasswordString)
    };
    InitializeConfigurationCommand command = new(payload);
    try
    {
      await Mediator.Send(command);
    }
    catch (Exception)
    {
    }
    Assert.Null(_cacheService.Configuration);
  }

  [Fact(DisplayName = "It should throw ConfigurationAlreadyInitializedException when the configuration is already initialized.")]
  public async Task It_should_throw_ConfigurationAlreadyInitializedException_when_the_configuration_is_already_initialized()
  {
    LocaleUnit defaultLocale = new(Faker.Locale);
    ActorId actorId = new();
    ConfigurationAggregate configuration = ConfigurationAggregate.Initialize(defaultLocale, actorId);
    await _configurationRepository.SaveAsync(configuration);

    InitializeConfigurationPayload payload = new()
    {
      DefaultLocale = defaultLocale.Code,
      User = new UserPayload(Faker.Person.UserName, PasswordString)
    };
    InitializeConfigurationCommand command = new(payload);
    await Assert.ThrowsAsync<ConfigurationAlreadyInitializedException>(async () => await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    InitializeConfigurationPayload payload = new()
    {
      DefaultLocale = "fr-US"
    };
    InitializeConfigurationCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("DefaultLocale", exception.Errors.Single().PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the user payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_user_payload_is_not_valid()
  {
    InitializeConfigurationPayload payload = new()
    {
      DefaultLocale = Faker.Locale,
      User = new UserPayload(Faker.Person.UserName, "AAaa!!11")
    };
    InitializeConfigurationCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("PasswordRequiresUniqueChars", exception.Errors.Single().ErrorCode);
  }
}
