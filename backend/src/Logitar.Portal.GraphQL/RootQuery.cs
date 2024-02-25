using GraphQL.Types;
using Logitar.Portal.GraphQL.Configurations;
using Logitar.Portal.GraphQL.Realms;

namespace Logitar.Portal.GraphQL;

internal class RootQuery : ObjectGraphType
{
  public RootQuery()
  {
    Name = nameof(RootQuery);

    //ApiKeyQueries.Register(this); // TODO(fpion): implement
    ConfigurationQueries.Register(this);
    //DictionaryQueries.Register(this); // TODO(fpion): implement
    //MessageQueries.Register(this); // TODO(fpion): implement
    RealmQueries.Register(this);
    //RoleQueries.Register(this); // TODO(fpion): implement
    //SenderQueries.Register(this); // TODO(fpion): implement
    //SessionQueries.Register(this); // TODO(fpion): implement
    //TemplateQueries.Register(this); // TODO(fpion): implement
    //UserQueries.Register(this); // TODO(fpion): implement

    // TODO(fpion): One-Time Passwords
  }
}
