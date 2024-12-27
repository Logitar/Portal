using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.Infrastructure;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Infrastructure.Caching;
using Logitar.Portal.Infrastructure.Messages.Providers;
using Logitar.Portal.Infrastructure.Messages.Providers.Mailgun;
using Logitar.Portal.Infrastructure.Messages.Providers.SendGrid;
using Logitar.Portal.Infrastructure.Messages.Providers.Twilio;
using Logitar.Portal.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Logitar.Portal.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarIdentityInfrastructure()
      .AddLogitarPortalApplication()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddMemoryCache()
      .AddSenderProviders()
      .AddSingleton(InitializeCachingSettings)
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer, EventSerializer>()
      .RemoveAll<IEventBus>().AddScoped<IEventBus, EventBus>();
  }

  private static IServiceCollection AddSenderProviders(this IServiceCollection services)
  {
    return services
      .AddSingleton<IProviderStrategy, MailgunStrategy>()
      .AddSingleton<IProviderStrategy, SendGridStrategy>()
      .AddSingleton<IProviderStrategy, TwilioStrategy>();
  }

  private static CachingSettings InitializeCachingSettings(IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection(CachingSettings.SectionKey).Get<CachingSettings>() ?? new();
  }
}
