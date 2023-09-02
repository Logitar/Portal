using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Infrastructure.Caching;
using Logitar.Portal.Infrastructure.Converters;
using Logitar.Portal.Infrastructure.Passwords;
using Logitar.Portal.Infrastructure.Passwords.Strategies;
using Logitar.Portal.Infrastructure.Tokens;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarPortalApplication()
      .AddPasswordStrategies()
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer>(serviceProvider => new EventSerializer(new JsonConverter[]
      {
        new GenderConverter(),
        new LocaleConverter(),
        new TimeZoneEntryConverter(),
        serviceProvider.GetRequiredService<PasswordConverter>()
      }))
      .AddSingleton<IPasswordService, PasswordService>()
      .AddSingleton<ITokenManager, JsonWebTokenManager>()
      .AddSingleton<PasswordConverter>()
      .AddScoped<IEventBus, EventBus>();
  }

  private static IServiceCollection AddPasswordStrategies(this IServiceCollection services)
  {
    services.AddSingleton<IPbkdf2Settings, Pbkdf2Settings>();

    return services.AddSingleton<IPasswordStrategy, Pbkdf2Strategy>();
  }
}
