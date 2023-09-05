using GraphQL.Types;
using Logitar.Portal.GraphQL.Configurations;
using Logitar.Portal.GraphQL.Realms;

namespace Logitar.Portal.GraphQL;

internal class RootQuery : ObjectGraphType
{
  public RootQuery()
  {
    Name = nameof(RootQuery);

    ConfigurationQueries.Register(this);
    RealmQueries.Register(this);
  }
}
