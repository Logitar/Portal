using Logitar.Portal.Core2;
using Logitar.Portal.Core2.Users;
using Logitar.Portal.Infrastructure2.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.Infrastructure2
{
  internal static class DependencyInjectionExtensions
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
        .AddScoped<IRepository, Repository>();
    }
  }
}
