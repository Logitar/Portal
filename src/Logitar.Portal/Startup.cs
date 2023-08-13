using Logitar.Portal.Extensions;

namespace Logitar.Portal;

public class Startup : StartupBase
{
  private readonly IConfiguration _configuration;

  private readonly bool _enableOpenApi;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;

    _enableOpenApi = configuration.GetValue<bool>("EnableOpenApi");
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddControllers();

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (_enableOpenApi)
    {
      builder.UseOpenApi();
    }

    builder.UseHttpsRedirection();

    if (builder is WebApplication application)
    {
      application.MapControllers();
    }
  }
}
