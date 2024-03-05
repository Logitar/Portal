﻿using Logitar.Portal.Application.Logging;
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
    MongoDBSettings? settings = configuration.GetSection("MongoDB").Get<MongoDBSettings>();
    return services.AddLogitarPortalMongoDB(settings);
  }
  public static IServiceCollection AddLogitarPortalMongoDB(this IServiceCollection services, MongoDBSettings? settings)
  {
    if (settings != null)
    {
      MongoClient client = new(settings.ConnectionString);
      IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
      services.AddSingleton(database).AddTransient<ILogRepository, LogRepository>();
    }

    return services.AddLogitarPortalInfrastructure();
  }
}
