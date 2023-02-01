using Logitar.Portal.Application;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Infrastructure.Queriers;
using Logitar.Portal.Infrastructure.Repositories;
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
        .AddAutoMapper(assembly)
        .AddDbContext<PortalContext>((provider, options) =>
        {
          IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
          options.UseNpgsql(configuration.GetValue<string>($"POSTGRESQLCONNSTR_{nameof(PortalContext)}"));
        })
        .AddMediatR(assembly)
        .AddQueriers()
        .AddRepositories()
        .AddSingleton<IUserValidator, CustomUserValidator>()
        .AddSingleton<IPasswordService, PasswordService>()
        .AddScoped<IMappingService, MappingService>()
        .AddTransient<IRequestPipeline, RequestPipeline>();
    }

    private static IServiceCollection AddQueriers(this IServiceCollection services)
    {
      return services.AddScoped<ISessionQuerier, SessionQuerier>();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
      return services
        .AddScoped<IRepository, Repository>()
        .AddScoped<IRealmRepository, RealmRepository>()
        .AddScoped<IUserRepository, UserRepository>();
    }
  }
}
