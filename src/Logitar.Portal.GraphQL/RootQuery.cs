using GraphQL.Types;
using Logitar.Portal.GraphQL.Configurations;

namespace Logitar.Portal.GraphQL;

internal class RootQuery : ObjectGraphType
{
  public RootQuery()
  {
    Name = nameof(RootQuery);

    ConfigurationQueries.Register(this);
  }
}
