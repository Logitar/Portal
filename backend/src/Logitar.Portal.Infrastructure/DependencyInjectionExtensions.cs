﻿using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.Infrastructure;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Infrastructure.Caching;
using Logitar.Portal.Infrastructure.Converters;
using Logitar.Portal.Infrastructure.Messages.Providers;
using Logitar.Portal.Infrastructure.Messages.Providers.Mailgun;
using Logitar.Portal.Infrastructure.Messages.Providers.SendGrid;
using Logitar.Portal.Infrastructure.Messages.Providers.Twilio;
using Logitar.Portal.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
      .AddSingleton<IEventSerializer>(serviceProvider => new EventSerializer(serviceProvider.GetLogitarPortalJsonConverters()))
      .AddTransient<IEventBus, PortalEventBus>();
  }

  private static IServiceCollection AddSenderProviders(this IServiceCollection services)
  {
    return services
      .AddSingleton<IProviderStrategy, MailgunStrategy>()
      .AddSingleton<IProviderStrategy, SendGridStrategy>()
      .AddSingleton<IProviderStrategy, TwilioStrategy>();
  }

  public static IEnumerable<JsonConverter> GetLogitarPortalJsonConverters(this IServiceProvider serviceProvider)
  {
    IEnumerable<JsonConverter> identityConverters = serviceProvider.GetLogitarIdentityJsonConverters();

    int capacity = identityConverters.Count() + 2;
    List<JsonConverter> converters = new(capacity);
    converters.AddRange(identityConverters);

    converters.Add(new ConfigurationIdConverter());
    converters.Add(new DictionaryIdConverter());
    converters.Add(new JwtSecretConverter());
    converters.Add(new MessageIdConverter());
    converters.Add(new RealmIdConverter());
    converters.Add(new SenderIdConverter());
    converters.Add(new SubjectConverter());
    converters.Add(new TemplateIdConverter());
    converters.Add(new UniqueKeyConverter());
    converters.Add(new UniqueSlugConverter());

    return converters;
  }

  private static CachingSettings InitializeCachingSettings(IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection("Caching").Get<CachingSettings>() ?? new();
  }
}
