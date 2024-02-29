using Bogus;
using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Configurations.Commands;
using Logitar.Portal.Application.Pipeline;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Logitar.Portal.EntityFrameworkCore.SqlServer;
using Logitar.Portal.Infrastructure.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

public abstract class IntegrationTests : IAsyncLifetime
{
  protected const string PasswordString = "P@s$W0rD";

  private readonly bool _initializeConfiguration;

  protected Faker Faker { get; } = new();

  protected IServiceProvider ServiceProvider { get; }
  protected IMediator Mediator { get; }
  protected IRequestPipeline Pipeline { get; }
  protected EventContext EventContext { get; }
  protected IdentityContext IdentityContext { get; }
  protected PortalContext PortalContext { get; }

  protected Uri BaseUri => ApplicationContext.BaseUri;

  protected Realm Realm { get; }
  protected TenantId TenantId => new(new AggregateId(Realm.Id).Value);

  private TestApplicationContext ApplicationContext => (TestApplicationContext)ServiceProvider.GetRequiredService<IApplicationContext>();

  protected IntegrationTests(bool initializeConfiguration = true)
  {
    _initializeConfiguration = initializeConfiguration;

    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .AddUserSecrets("f5e49206-87cc-4c7a-8962-e79ebd5c850c")
      .Build();

    string connectionString = configuration.GetValue<string>("SQLCONNSTR_Portal")
      ?.Replace("{Database}", GetType().Name) ?? string.Empty;

    ServiceProvider = new ServiceCollection()
      .AddSingleton(configuration)
      .AddLogitarPortalWithEntityFrameworkCoreSqlServer(connectionString)
      .AddSingleton<IApplicationContext, TestApplicationContext>()
      .BuildServiceProvider();

    Mediator = ServiceProvider.GetRequiredService<IMediator>();
    Pipeline = ServiceProvider.GetRequiredService<IRequestPipeline>();
    EventContext = ServiceProvider.GetRequiredService<EventContext>();
    IdentityContext = ServiceProvider.GetRequiredService<IdentityContext>();
    PortalContext = ServiceProvider.GetRequiredService<PortalContext>();

    Realm = new("tests", JwtSecretUnit.Generate().Value)
    {
      Id = Guid.NewGuid(),
      DisplayName = "Tests",
      DefaultLocale = new Locale(Faker.Locale),
      Url = $"https://www.{Faker.Internet.DomainName()}",
      RequireUniqueEmail = true
    };
  }

  public virtual async Task InitializeAsync()
  {
    IPublisher publisher = ServiceProvider.GetRequiredService<IPublisher>();
    await publisher.Publish(new InitializeDatabaseCommand());

    TableId[] tables = [EventDb.Events.Table, IdentityDb.Actors.Table, IdentityDb.CustomAttributes.Table, IdentityDb.Sessions.Table, IdentityDb.Users.Table];
    foreach (TableId table in tables)
    {
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    if (_initializeConfiguration)
    {
      UserPayload user = new(Faker.Person.UserName, PasswordString)
      {
        EmailAddress = Faker.Person.Email,
        FirstName = Faker.Person.FirstName,
        LastName = Faker.Person.LastName
      };
      SessionPayload session = new();
      session.CustomAttributes.Add(new CustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}"));
      session.CustomAttributes.Add(new CustomAttribute("IpAddress", Faker.Internet.Ip()));
      InitializeConfigurationPayload payload = new(user, session)
      {
        DefaultLocale = Faker.Locale
      };
      InitializeConfigurationCommand command = new(payload);
      await Mediator.Send(command);
    }
  }

  public virtual Task DisposeAsync() => Task.CompletedTask;

  protected void SetActor(Actor actor) => ApplicationContext.Actor = actor;

  protected void SetRealm() => SetRealm(Realm);
  protected void SetRealm(Realm? realm) => ApplicationContext.Realm = realm;
}
