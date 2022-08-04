using Logitar.Portal.Client.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Client
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddPortalClient(this IServiceCollection services, IConfiguration configuration)
    {
      ArgumentNullException.ThrowIfNull(services);
      ArgumentNullException.ThrowIfNull(configuration);

      var settings = configuration.GetSection("Portal").Get<PortalSettings>();

      return services.AddPortalClient(settings);
    }

    public static IServiceCollection AddPortalClient(this IServiceCollection services, PortalSettings? settings = null)
    {
      ArgumentNullException.ThrowIfNull(services);

      if (settings != null)
      {
        services.Configure<PortalSettings>(options =>
        {
          options.ApiKey = settings.ApiKey;
          options.BaseUrl = settings.BaseUrl;
        });
      }

      return services
        .AddHttpClient()
        .AddScoped<IAccountService, AccountService>()
        .AddScoped<IApiKeyService, ApiKeyService>()
        .AddScoped<IMessageService, MessageService>()
        .AddScoped<IRealmService, RealmService>()
        .AddScoped<ISenderService, SenderService>()
        .AddScoped<ITemplateService, TemplateService>()
        .AddScoped<ITokenService, TokenService>()
        .AddScoped<IUserService, UserService>();
    }
  }
}
