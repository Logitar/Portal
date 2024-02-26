using GraphQL;
using GraphQL.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.GraphQL;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalGraphQL(this IServiceCollection services, IConfiguration configuration)
  {
    GraphQLSettings settings = configuration.GetSection("GraphQL").Get<GraphQLSettings>() ?? new();

    return services.AddLogitarPortalGraphQL(settings);
  }
  public static IServiceCollection AddLogitarPortalGraphQL(this IServiceCollection services, GraphQLSettings settings)
  {
    return services.AddGraphQL(builder => builder
      .AddAuthorizationRule()
      .AddSchema<PortalSchema>()
      .AddSystemTextJson()
      .AddErrorInfoProvider(new ErrorInfoProvider(options => options.ExposeExceptionDetails = settings.ExposeExceptionDetails))
      .AddGraphTypes(typeof(PortalSchema).Assembly)
      .ConfigureExecutionOptions(options => options.EnableMetrics = settings.EnableMetrics));
  }
}
