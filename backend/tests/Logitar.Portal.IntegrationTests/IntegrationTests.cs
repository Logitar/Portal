using Bogus;
using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching.Commands;
using Logitar.Portal.Application.Configurations.Commands;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Users;
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
  protected EventContext EventContext { get; }
  protected IdentityContext IdentityContext { get; }
  protected PortalContext PortalContext { get; }

  protected IntegrationTests(bool initializeConfiguration = true)
  {
    _initializeConfiguration = initializeConfiguration;

    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    string connectionString = configuration.GetValue<string>("SQLCONNSTR_Portal")
      ?.Replace("{Database}", GetType().Name) ?? string.Empty;

    ServiceProvider = new ServiceCollection()
      .AddSingleton(configuration)
      .AddLogitarPortalWithEntityFrameworkCoreSqlServer(connectionString)
      .AddSingleton<IApplicationContext, TestApplicationContext>()
      .BuildServiceProvider();

    Mediator = ServiceProvider.GetRequiredService<IMediator>();
    EventContext = ServiceProvider.GetRequiredService<EventContext>();
    IdentityContext = ServiceProvider.GetRequiredService<IdentityContext>();
    PortalContext = ServiceProvider.GetRequiredService<PortalContext>();
  }

  public virtual async Task InitializeAsync()
  {
    IPublisher publisher = ServiceProvider.GetRequiredService<IPublisher>();
    await publisher.Publish(new InitializeDatabaseCommand());

    TableId[] tables = [EventDb.Events.Table];
    foreach (TableId table in tables)
    {
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    if (_initializeConfiguration)
    {
      UserPayload user = new(Faker.Person.UserName, PasswordString)
      {
        Email = new EmailPayload(Faker.Person.Email, isVerified: false),
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

    await publisher.Publish(new InitializeCachingCommand());
  }

  public virtual Task DisposeAsync() => Task.CompletedTask;
}
