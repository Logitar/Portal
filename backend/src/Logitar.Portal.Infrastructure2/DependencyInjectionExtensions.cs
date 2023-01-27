using Logitar.Portal.Core2;
using Logitar.Portal.Core2.Realms;
using Logitar.Portal.Core2.Sessions;
using Logitar.Portal.Core2.Users;
using Logitar.Portal.Infrastructure2.Queriers;
using Logitar.Portal.Infrastructure2.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.Infrastructure2
{
  public static class DependencyInjectionExtensions
  {
    public static IServiceCollection AddLogitarPortalInfrastructure2(this IServiceCollection services)
    {
      Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

      return services
        .AddDbContext<PortalContext>((provider, options) =>
        {
          IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
          options.UseNpgsql(configuration.GetValue<string>($"POSTGRESQLCONNSTR_{nameof(PortalContext)}"));
        })
        .AddMediatR(assembly)
        .AddSingleton<IPasswordService, PasswordService>()
        .AddSingleton<IUserValidator, CustomUserValidator>()
        .AddScoped<IRequestPipeline, RequestPipeline>()
        .AddScoped<IRepository, Repository>();
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
