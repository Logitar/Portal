﻿using GraphQL.Types;
using Logitar.Portal.GraphQL.ApiKeys;
using Logitar.Portal.GraphQL.Configurations;
using Logitar.Portal.GraphQL.Realms;
using Logitar.Portal.GraphQL.Roles;
using Logitar.Portal.GraphQL.Sessions;
using Logitar.Portal.GraphQL.Users;

namespace Logitar.Portal.GraphQL;

internal class RootQuery : ObjectGraphType
{
  public RootQuery()
  {
    Name = nameof(RootQuery);

    ApiKeyQueries.Register(this);
    ConfigurationQueries.Register(this);
    RealmQueries.Register(this);
    RoleQueries.Register(this);
    SessionQueries.Register(this);
    UserQueries.Register(this);
  }
}
