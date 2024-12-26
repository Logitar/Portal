using GraphQL;
using GraphQL.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.GraphQL;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalGraphQL(this IServiceCollection services, IConfiguration configuration)
  {
    GraphQLSettings settings = configuration.GetSection(GraphQLSettings.SectionKey).Get<GraphQLSettings>() ?? new();
    return services.AddLogitarPortalGraphQL(settings);
  }
  public static IServiceCollection AddLogitarPortalGraphQL(this IServiceCollection services, IGraphQLSettings settings)
  {
    return services.AddGraphQL(builder => builder
      .AddAuthorizationRule()
      .AddSchema<PortalSchema>()
      .AddSystemTextJson()
      .AddErrorInfoProvider(new ErrorInfoProvider(options => options.ExposeExceptionDetails = settings.ExposeExceptionDetails))
      .AddGraphTypes(Assembly.GetExecutingAssembly())
      .ConfigureExecutionOptions(options => options.EnableMetrics = settings.EnableMetrics));
  }
}
