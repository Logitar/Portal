﻿using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portal.Core;
using Portal.Core.ApiKeys;
using Portal.Core.Emails.Messages;
using Portal.Core.Emails.Providers;
using Portal.Core.Emails.Senders;
using Portal.Core.Emails.Templates;
using Portal.Core.Realms;
using Portal.Core.Sessions;
using Portal.Core.Tokens;
using Portal.Core.Users;
using Portal.Infrastructure.Emails.Messages;
using Portal.Infrastructure.Queriers;
using Portal.Infrastructure.Repositories;
using Portal.Infrastructure.Settings;
using Portal.Infrastructure.Tokens;
using Portal.Infrastructure.Users;
using System.Reflection;

using Providers = Portal.Infrastructure.Emails.Providers;

namespace Portal.Infrastructure
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddPortalInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
      Assembly assembly = typeof(ServiceCollectionExtensions).Assembly;

      return services
        .AddDbContext<PortalDbContext>()
        .AddSingleton(configuration.GetSection("Jwt").Get<JwtSettings>() ?? new())
        .AddSingleton<IPasswordService, PasswordService>()
        .AddSingleton<ITemplateCompiler, TemplateCompiler>()
        .AddScoped<IDatabaseService, DatabaseService>()
        .AddScoped<IJwtBlacklist, JwtBlacklist>()
        .AddScoped<ISecurityTokenService, JwtService>()
        .AddProviderStrategies()
        .AddQueriers()
        .AddRepositories()
        .AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
    }

    private static IServiceCollection AddProviderStrategies(this IServiceCollection services)
    {
      return services
        .AddScoped<ISendGridStrategy, Providers.SendGrid.SendGridStrategy>();
    }

    private static IServiceCollection AddQueriers(this IServiceCollection services)
    {
      return services
        .AddScoped<IApiKeyQuerier, ApiKeyQuerier>()
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
        .AddScoped<IRepository<ApiKey>, Repository<ApiKey>>()
        .AddScoped<IRepository<Message>, Repository<Message>>()
        .AddScoped<IRepository<Realm>, Repository<Realm>>()
        .AddScoped<IRepository<Sender>, Repository<Sender>>()
        .AddScoped<IRepository<Session>, Repository<Session>>()
        .AddScoped<IRepository<Template>, Repository<Template>>()
        .AddScoped<IRepository<User>, Repository<User>>();
    }
  }
}
