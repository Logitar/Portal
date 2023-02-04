using FluentValidation;
using Logitar.Portal.Application;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Application.Messages;
using Logitar.Portal.Application.Messages.Providers;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Templates;
using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Infrastructure.Messages;
using Logitar.Portal.Infrastructure.Messages.Providers.SendGrid;
using Logitar.Portal.Infrastructure.Queriers;
using Logitar.Portal.Infrastructure.Tokens;
using Logitar.Portal.Infrastructure.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.Infrastructure
{
  public static class DependencyInjectionExtensions
  {
    public static IServiceCollection AddLogitarPortalInfrastructure(this IServiceCollection services)
    {
      Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

      return services
        .AddAutoMapper(assembly)
        .AddDbContext<PortalContext>((provider, options) =>
        {
          IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
          options.UseNpgsql(configuration.GetValue<string>($"POSTGRESQLCONNSTR_{nameof(PortalContext)}"));
        })
        .AddMediatR(assembly)
        .AddProviderStrategies()
        .AddQueriers()
        .AddSingleton<IPasswordService, PasswordService>()
        .AddSingleton<ITemplateCompiler, TemplateCompiler>()
        .AddScoped<IJwtBlacklist, JwtBlacklist>()
        .AddScoped<IRepository, Repository>()
        .AddScoped<IRequestPipeline, RequestPipeline>()
        .AddScoped<ISecurityTokenService, JwtService>()
        .AddScoped<IUserValidator, CustomUserValidator>();
    }

    private static IServiceCollection AddProviderStrategies(this IServiceCollection services)
    {
      return services
        .AddSingleton<IValidator<SendGridSettings>, SendGridSettingsValidator>()
        .AddScoped<ISendGridStrategy, SendGridStrategy>();
    }

    private static IServiceCollection AddQueriers(this IServiceCollection services)
    {
      return services
        .AddScoped<IApiKeyQuerier, ApiKeyQuerier>()
        .AddScoped<IDictionaryQuerier, DictionaryQuerier>()
        .AddScoped<IMessageQuerier, MessageQuerier>()
        .AddScoped<IRealmQuerier, RealmQuerier>()
        .AddScoped<ISenderQuerier, SenderQuerier>()
        .AddScoped<ISessionQuerier, SessionQuerier>()
        .AddScoped<ITemplateQuerier, TemplateQuerier>()
        .AddScoped<IUserQuerier, UserQuerier>();
    }
  }
}
