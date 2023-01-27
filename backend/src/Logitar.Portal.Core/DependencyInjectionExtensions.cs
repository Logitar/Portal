using FluentValidation;
using Logitar.Portal.Core.Accounts;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Users;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.Core
{
  public static class DependencyInjectionExtensions
  {
    public static IServiceCollection AddLogitarPortalCore(this IServiceCollection services)
    {
      Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

      return services
        .AddMediatR(assembly)
        .AddValidatorsFromAssembly(assembly, includeInternalTypes: true)
        .AddTransient<IAccountService, AccountService>()
        .AddTransient<IConfigurationService, ConfigurationService>()
        .AddTransient<IRealmService, RealmService>()
        .AddTransient<ISessionService, SessionService>()
        .AddTransient<ISignInService, SignInService>()
        .AddTransient<IUserService, UserService>();
    }
  }
}
