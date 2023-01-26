using FluentValidation;
using Logitar.Portal.Core2.Configurations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.Core2
{
  internal static class DependencyInjectionExtensions
  {
    public static IServiceCollection AddLogitarPortalCore(this IServiceCollection services)
    {
      Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

      return services
        .AddMediatR(assembly)
        .AddValidatorsFromAssembly(assembly, includeInternalTypes: true)
        .AddTransient<IConfigurationService, ConfigurationService>();
    }
  }
}
