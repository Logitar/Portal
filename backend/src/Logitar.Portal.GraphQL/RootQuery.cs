using GraphQL.Types;
using Logitar.Portal.GraphQL.ApiKeys;
using Logitar.Portal.GraphQL.Configurations;
using Logitar.Portal.GraphQL.Dictionaries;
using Logitar.Portal.GraphQL.Messages;
using Logitar.Portal.GraphQL.Passwords;
using Logitar.Portal.GraphQL.Realms;
using Logitar.Portal.GraphQL.Roles;
using Logitar.Portal.GraphQL.Senders;
using Logitar.Portal.GraphQL.Sessions;
using Logitar.Portal.GraphQL.Templates;
using Logitar.Portal.GraphQL.Users;

namespace Logitar.Portal.GraphQL;

internal class RootQuery : ObjectGraphType
{
  public RootQuery()
  {
    Name = nameof(RootQuery);

    ApiKeyQueries.Register(this);
    ConfigurationQueries.Register(this);
    DictionaryQueries.Register(this);
    MessageQueries.Register(this);
    OneTimePasswordQueries.Register(this);
    RealmQueries.Register(this);
    RoleQueries.Register(this);
    SenderQueries.Register(this);
    SessionQueries.Register(this);
    TemplateQueries.Register(this);
    UserQueries.Register(this);
  }
}
