using GraphQL;
using Logitar.Cms.Schema.Extensions;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Constants;

namespace Logitar.Portal.GraphQL.Configurations;

internal static class ConfigurationQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<ConfigurationGraphType>("configuration")
      .AuthorizeWithPolicy(Policies.PortalActor)
      .Description("Retrieves the configuration of the system.")
      .ResolveAsync(async context => await context.GetRequiredService<IConfigurationService, object?>().ReadAsync(context.CancellationToken));
  }
}
