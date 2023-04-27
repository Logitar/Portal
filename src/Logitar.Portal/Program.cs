using Logitar.Portal.Core.Caching;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Commands;
using MediatR;

namespace Logitar.Portal;

public class Program
{
  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseStaticWebAssets();

    Startup startup = new(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    using IServiceScope scope = application.Services.CreateScope();

    #region TODO(fpion): refactor
    if (application.Configuration.GetValue<bool>("MigrateDatabase"))
    {
      IRequest? migrateDatabase = null;
      DatabaseProvider databaseProvider = application.Configuration.GetValue<DatabaseProvider>("DatabaseProvider");
      switch (databaseProvider)
      {
        case DatabaseProvider.EntityFrameworkCorePostgreSQL:
          migrateDatabase = new MigrateDatabase();
          break;
      }

      if (migrateDatabase != null)
      {
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(migrateDatabase);
      }
    }

    ICacheService cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
    IConfigurationRepository configurationRepository = scope.ServiceProvider.GetRequiredService<IConfigurationRepository>();
    ConfigurationAggregate? configuration = await configurationRepository.LoadAsync();
    cacheService.Configuration = configuration;
    #endregion

    application.Run();
  }
}
