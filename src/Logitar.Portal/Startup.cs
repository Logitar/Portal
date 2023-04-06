using Logitar.Portal.v2.Core;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.v2.Web;

namespace Logitar.Portal;

public class Startup : StartupBase
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  /// <summary>
  /// TODO(fpion): refactor
  /// </summary>
  /// <param name="services"></param>
  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="DatabaseProviderNotSupportedException"></exception>
  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddLogitarPortalV2Core();
    services.AddLogitarPortalv2Web();

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider>("DatabaseProvider");
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        string connectionString = _configuration.GetValue<string>("POSTGRESQLCONNSTR_PortalContext")
          ?? throw new InvalidOperationException("The configuration 'POSTGRESQLCONNSTR_PortalContext' could not be found.");
        services.AddLogitarPortalv2EntityFrameworkCorePostgreSQL(connectionString);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
  }

  /// <summary>
  /// TODO(fpion): refactor
  /// </summary>
  /// <param name="builder"></param>
  public override void Configure(IApplicationBuilder builder)
  {
    if (builder is WebApplication application)
    {
      if (application.Environment.IsDevelopment())
      {
        application.UseSwagger();
        application.UseSwaggerUI();
      }

      application.UseHttpsRedirection();
      application.UseStaticFiles();
      application.UseAuthentication();
      application.UseAuthorization();

      application.MapControllers();
    }
  }
}
