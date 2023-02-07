﻿using Logitar.Portal.Application;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Logitar.Portal.Web
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

      WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

      Startup startup = new();
      startup.ConfigureServices(builder.Services);

      WebApplication application = builder.Build();

      startup.Configure(application);

      if (application.Configuration.GetValue<bool>("MigrateDatabase"))
      {
        using IServiceScope scope = application.Services.CreateScope();
        using PortalContext context = scope.ServiceProvider.GetRequiredService<PortalContext>();
        await context.Database.MigrateAsync();

        IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();
        Configuration? configuration = await repository.LoadConfigurationAsync();
        if (configuration != null)
        {
          ICacheService cacheService = application.Services.GetRequiredService<ICacheService>();
          cacheService.Configuration = configuration;
        }
      }

      application.Run();
    }
  }
}
