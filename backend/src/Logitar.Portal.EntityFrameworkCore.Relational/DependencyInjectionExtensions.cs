using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Application.Messages;
using Logitar.Portal.Application.Passwords;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Templates;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Queriers;
using Logitar.Portal.EntityFrameworkCore.Relational.Repositories;
using Logitar.Portal.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalWithEntityFrameworkCoreRelational(this IServiceCollection services)
  {
    return services
      .AddLogitarIdentityWithEntityFrameworkCoreRelational()
      .AddLogitarPortalInfrastructure()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddQueriers()
      .AddRepositories()
      .AddTransient<IActorService, ActorService>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddTransient<IApiKeyQuerier, ApiKeyQuerier>()
      .AddTransient<IConfigurationQuerier, ConfigurationQuerier>()
      .AddTransient<IDictionaryQuerier, DictionaryQuerier>()
      .AddTransient<IMessageQuerier, MessageQuerier>()
      .AddTransient<IOneTimePasswordQuerier, OneTimePasswordQuerier>()
      .AddTransient<IRealmQuerier, RealmQuerier>()
      .AddTransient<IRoleQuerier, RoleQuerier>()
      .AddTransient<ISenderQuerier, SenderQuerier>()
      .AddTransient<ISessionQuerier, SessionQuerier>()
      .AddTransient<ITemplateQuerier, TemplateQuerier>()
      .AddTransient<IUserQuerier, UserQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationRepository, ConfigurationRepository>()
      .AddTransient<IDictionaryRepository, DictionaryRepository>()
      .AddTransient<ILogRepository, LogRepository>()
      .AddTransient<IMessageRepository, MessageRepository>()
      .AddTransient<IRealmRepository, RealmRepository>()
      .AddTransient<ISenderRepository, SenderRepository>()
      .AddTransient<ITemplateRepository, TemplateRepository>();
  }
}
