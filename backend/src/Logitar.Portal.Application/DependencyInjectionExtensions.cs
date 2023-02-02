using FluentValidation;
using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Application.Users;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.Application
{
  public static class DependencyInjectionExtensions
  {
    public static IServiceCollection AddLogitarPortalApplication(this IServiceCollection services)
    {
      Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

      return services
        .AddApplicationServices()
        .AddMediatR(assembly)
        .AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
      return services
        .AddTransient<IAccountService, AccountService>()
        .AddTransient<IConfigurationService, ConfigurationService>()
        .AddTransient<IGoogleService, GoogleService>()
        .AddTransient<ISessionService, SessionService>()
        .AddTransient<ISignInService, SignInService>()
        .AddTransient<ITokenService, TokenService>()
        .AddTransient<IUserService, UserService>();
    }
  }
}
