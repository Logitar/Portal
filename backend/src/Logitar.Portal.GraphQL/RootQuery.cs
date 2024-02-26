using GraphQL.Types;
using Logitar.Portal.GraphQL.ApiKeys;
using Logitar.Portal.GraphQL.Configurations;
using Logitar.Portal.GraphQL.Realms;
using Logitar.Portal.GraphQL.Roles;
using Logitar.Portal.GraphQL.Users;

namespace Logitar.Portal.GraphQL;

internal class RootQuery : ObjectGraphType
{
  public RootQuery()
  {
    Name = nameof(RootQuery);

    ApiKeyQueries.Register(this);
    ConfigurationQueries.Register(this);
    //DictionaryQueries.Register(this); // TODO(fpion): implement
    //MessageQueries.Register(this); // TODO(fpion): implement
    RealmQueries.Register(this);
    RoleQueries.Register(this);
    //SenderQueries.Register(this); // TODO(fpion): implement
    //SessionQueries.Register(this); // TODO(fpion): implement
    //TemplateQueries.Register(this); // TODO(fpion): implement
    UserQueries.Register(this);

    // TODO(fpion): One-Time Passwords
  }
}
