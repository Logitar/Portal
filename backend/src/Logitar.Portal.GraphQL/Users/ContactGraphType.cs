using GraphQL.Types;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.GraphQL.Actors;

namespace Logitar.Portal.GraphQL.Users;

internal abstract class ContactGraphType<T> : ObjectGraphType<T> where T : Contact
{
  protected ContactGraphType(string? description = null)
  {
    Name = typeof(T).Name;
    Description = description;

    Field(x => x.IsVerified)
      .Description("A value indicating whether or not the contact is verified.");
    Field(x => x.VerifiedBy, type: typeof(ActorGraphType))
      .Description("The actor who verified the contact.");
    Field(x => x.VerifiedOn, nullable: true)
      .Description("The date and time when the contact was verified.");
  }
}
