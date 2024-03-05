using Logitar.Portal.Application;
using Logitar.Portal.EntityFrameworkCore.SqlServer;
using Logitar.Portal.Infrastructure;
using Logitar.Portal.MassTransit;
using Logitar.Portal.Worker.Settings;
using Logitar.Portal.Worker.Tasks;

namespace Logitar.Portal.Worker;

internal class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public virtual void ConfigureServices(IServiceCollection services)
  {
    services.AddLogitarPortalMassTransit(_configuration);

    services.AddApplicationInsightsTelemetryWorkerService();

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider")
     ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddLogitarPortalWithEntityFrameworkCoreSqlServer(_configuration);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }

    services.AddSingleton<IBaseUrl, WorkerBaseUrl>();

    services.AddSingleton(_configuration.GetSection("Cron").Get<CronSettings>() ?? new());
    services.AddHostedService<PurgeTokenBlacklist>();
  }
}
