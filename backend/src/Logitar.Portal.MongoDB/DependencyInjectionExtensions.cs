using Logitar.Portal.Application.Logging;
using Logitar.Portal.Infrastructure;
using Logitar.Portal.MongoDB.Repositories;
using Logitar.Portal.MongoDB.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Logitar.Portal.MongoDB;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalMongoDB(this IServiceCollection services, IConfiguration configuration)
  {
    MongoDBSettings settings = configuration.GetSection("MongoDB").Get<MongoDBSettings>() ?? new();
    return services.AddLogitarPortalMongoDB(settings);
  }
  public static IServiceCollection AddLogitarPortalMongoDB(this IServiceCollection services, MongoDBSettings settings)
  {
    if (!string.IsNullOrWhiteSpace(settings.ConnectionString) && !string.IsNullOrWhiteSpace(settings.DatabaseName))
    {
      MongoClient client = new(settings.ConnectionString.Trim());
      IMongoDatabase database = client.GetDatabase(settings.DatabaseName.Trim());
      services.AddSingleton(database).AddTransient<ILogRepository, LogRepository>();
    }

    return services.AddLogitarPortalInfrastructure();
  }
}
