using Logitar.Portal.Core;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Users;
using Logitar.Portal.Infrastructure.Queriers;
using Logitar.Portal.Infrastructure.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.Infrastructure
{
  public static class DependencyInjectionExtensions
  {
    public static IServiceCollection AddLogitarPortalInfrastructure(this IServiceCollection services)
    {
      Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

      return services
        .AddDbContext<PortalContext>((provider, options) =>
        {
          IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
          options.UseNpgsql(configuration.GetValue<string>($"POSTGRESQLCONNSTR_{nameof(PortalContext)}"));
        })
        .AddMediatR(assembly)
        .AddScoped<IPasswordService, PasswordService>()
        .AddScoped<IRepository, Repository>()
        .AddScoped<IRequestPipeline, RequestPipeline>()
        .AddScoped<IUserValidator, CustomUserValidator>();
    }

    private static IServiceCollection AddQueriers(this IServiceCollection services)
    {
      return services
        .AddScoped<IRealmQuerier, RealmQuerier>()
        .AddScoped<ISessionQuerier, SessionQuerier>()
        .AddScoped<IUserQuerier, UserQuerier>();
    }
  }
}
