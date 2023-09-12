using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Logging;
using Logitar.Portal.EntityFrameworkCore.Relational.Queriers;
using Logitar.Portal.EntityFrameworkCore.Relational.Repositories;
using Logitar.Portal.EntityFrameworkCore.Relational.Tokens;
using Logitar.Portal.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalWithEntityFrameworkCoreRelational(this IServiceCollection services)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddLogitarPortalInfrastructure()
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddQueriers()
      .AddRepositories()
      .AddScoped<IActorService, ActorService>()
      .AddScoped<ILoggingService, LoggingService>()
      .AddScoped<ITokenBlacklist, TokenBlacklist>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<IApiKeyQuerier, ApiKeyQuerier>()
      .AddScoped<IConfigurationQuerier, ConfigurationQuerier>()
      .AddScoped<IDictionaryQuerier, DictionaryQuerier>()
      .AddScoped<IRealmQuerier, RealmQuerier>()
      .AddScoped<IRoleQuerier, RoleQuerier>()
      .AddScoped<ISenderQuerier, SenderQuerier>()
      .AddScoped<ISessionQuerier, SessionQuerier>()
      .AddScoped<IUserQuerier, UserQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<IApiKeyRepository, ApiKeyRepository>()
      .AddScoped<IConfigurationRepository, ConfigurationRepository>()
      .AddScoped<IDictionaryRepository, DictionaryRepository>()
      .AddScoped<IRealmRepository, RealmRepository>()
      .AddScoped<IRoleRepository, RoleRepository>()
      .AddScoped<ISenderRepository, SenderRepository>()
      .AddScoped<ISessionRepository, SessionRepository>()
      .AddScoped<IUserRepository, UserRepository>();
  }
}
