using GraphQL;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.GraphQL.Configurations;

internal static class ConfigurationQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<ConfigurationGraphType>("configuration")
      .Authorize()
      .Description("Retrieves the configuration of the system.")
      .ResolveAsync(async context => await context.GetQueryService<IConfigurationService, object?>().ReadAsync(
        context.CancellationToken
      ));
  }
}
