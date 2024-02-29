using Logitar.Portal.ApiKeys;
using Logitar.Portal.Client;
using Logitar.Portal.Client.ApiKeys;
using Logitar.Portal.Configurations;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Realms;
using Logitar.Portal.Roles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

internal class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public virtual void ConfigureServices(IServiceCollection services)
  {
    PortalSettings settings = _configuration.GetSection("Portal").Get<PortalSettings>() ?? new();
    StaticPortalSettings.Instance.BaseUrl = settings.BaseUrl;
    services.AddLogitarPortalClient(StaticPortalSettings.Instance);
    services.AddTransient<IApiKeyClient, ApiKeyClient>(); // NOTE(fpion): required since we inject it two times to create a Portal API key.

    services.AddTransient<ApiKeyClientTests>();
    services.AddTransient<ConfigurationClientTests>();
    services.AddTransient<CreateApiKeyTests>();
    services.AddTransient<InitializeConfigurationTests>();
    services.AddTransient<RealmClientTests>();
    services.AddTransient<RoleClientTests>();
  }
}
