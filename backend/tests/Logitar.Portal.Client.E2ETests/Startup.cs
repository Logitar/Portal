using Logitar.Portal.Client;
using Logitar.Portal.Configurations;
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
    services.AddLogitarPortalClient(_configuration);
    services.AddTransient<InitializeConfigurationTests>();
    services.AddTransient<ConfigurationClientTests>();
  }
}
