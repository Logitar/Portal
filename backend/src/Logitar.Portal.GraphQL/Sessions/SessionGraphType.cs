using GraphQL.Types;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.GraphQL.Actors;
using Logitar.Portal.GraphQL.Users;

namespace Logitar.Portal.GraphQL.Sessions;

internal class SessionGraphType : AggregateGraphType<Session>
{
  public SessionGraphType() : base("Represents an user session. Sessions are used to control & audit user access to resources.")
  {
    Field(x => x.IsPersistent)
      .Description("A value indicating whether or not the session is persistent.");
    Field(x => x.RefreshToken, nullable: true)
      .Description("The token used to renew the session.");

    Field(x => x.IsActive)
      .Description("A value indicating whether or not the session is active. Sessions are active until they are signed-out.");
    Field(x => x.SignedOutBy, type: typeof(ActorGraphType))
      .Description("The actor who signed-out the session.");
    Field(x => x.SignedOutOn, nullable: true)
      .Description("The date and time when the session was signed-out.");

    Field(x => x.CustomAttributes, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<CustomAttributeGraphType>>>))
      .Description("The custom attributes of the session.");

    Field(x => x.User, type: typeof(NonNullGraphType<UserGraphType>))
      .Description("The user owning the session.");
  }
}
