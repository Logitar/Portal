using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Core.Dictionaries;
using Logitar.Portal.Core.Logging;
using Logitar.Portal.Core.Messages;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Senders;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Templates;
using Logitar.Portal.Core.Tokens;
using Logitar.Portal.Core.Users;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Converters;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Logging;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Queriers;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Repositories;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL;

public static class DependencyInjectionExtensions
{
  private const string ConnectionStringKey = "POSTGRESQLCONNSTR_PortalContext";

  public static IServiceCollection AddLogitarPortalEntityFrameworkCorePostgreSQLStore(this IServiceCollection services, IConfiguration configuration)
  {
    string connectionString = configuration.GetValue<string>(ConnectionStringKey)
      ?? throw new InvalidOperationException($"The configuration key '{ConnectionStringKey}' could not be found.");

    return services.AddLogitarPortalEntityFrameworkCorePostgreSQLStore(connectionString);
  }

  public static IServiceCollection AddLogitarPortalEntityFrameworkCorePostgreSQLStore(this IServiceCollection services, string connectionString)
  {
    EventSerializer.Instance.RegisterConverter(new GenderConverter());
    EventSerializer.Instance.RegisterConverter(new Pbkdf2Converter());
    EventSerializer.Instance.RegisterConverter(new TimeZoneEntryConverter());

    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddAutoMapper(assembly)
      .AddDbContext<PortalContext>(options => options.UseNpgsql(connectionString))
      .AddEventSourcingWithEntityFrameworkCorePostgreSQL(connectionString)
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddQueriers()
      .AddRepositories()
      .AddScoped<IActorService, ActorService>()
      .AddScoped<IEventBus, EventBus>()
      .AddScoped<ILoggingService, LoggingService>()
      .AddScoped<ITokenBlacklist, TokenBlacklist>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<IDictionaryQuerier, DictionaryQuerier>()
      .AddScoped<IMessageQuerier, MessageQuerier>()
      .AddScoped<IRealmQuerier, RealmQuerier>()
      .AddScoped<ISenderQuerier, SenderQuerier>()
      .AddScoped<ISessionQuerier, SessionQuerier>()
      .AddScoped<ITemplateQuerier, TemplateQuerier>()
      .AddScoped<IUserQuerier, UserQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<IConfigurationRepository, ConfigurationRepository>()
      .AddScoped<IDictionaryRepository, DictionaryRepository>()
      .AddScoped<IMessageRepository, MessageRepository>()
      .AddScoped<IRealmRepository, RealmRepository>()
      .AddScoped<ISenderRepository, SenderRepository>()
      .AddScoped<ISessionRepository, SessionRepository>()
      .AddScoped<ITemplateRepository, TemplateRepository>()
      .AddScoped<IUserRepository, UserRepository>();
  }
}
