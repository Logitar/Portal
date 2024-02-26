using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.GraphQL;

public class PortalSchema : Schema
{
  public PortalSchema(IServiceProvider serviceProvider) : base(serviceProvider)
  {
    Query = serviceProvider.GetRequiredService<RootQuery>();
  }
}
