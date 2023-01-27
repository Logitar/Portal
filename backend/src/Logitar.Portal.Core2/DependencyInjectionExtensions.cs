using FluentValidation;
using Logitar.Portal.Core2.Accounts;
using Logitar.Portal.Core2.Configurations;
using Logitar.Portal.Core2.Sessions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.Core2
{
  public static class DependencyInjectionExtensions
  {
    public static IServiceCollection AddLogitarPortalCore2(this IServiceCollection services)
    {
      Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

      return services
        .AddMediatR(assembly)
        .AddValidatorsFromAssembly(assembly, includeInternalTypes: true)
        .AddTransient<IAccountService, AccountService>()
        .AddTransient<IConfigurationService, ConfigurationService>()
        .AddTransient<ISignInService, SignInService>();
    }
  }
}
