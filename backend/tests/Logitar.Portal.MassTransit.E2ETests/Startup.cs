using Logitar.Portal.MassTransit.Messages;

namespace Logitar.Portal.MassTransit;

internal class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public virtual void ConfigureServices(IServiceCollection services)
  {
    services.AddHostedService<Worker>();
    services.AddLogitarPortalMassTransit(_configuration);
    services.AddSingleton<TestContext>();

    services.AddSingleton<ITests, SendMessageCommandTests>();
  }
}
