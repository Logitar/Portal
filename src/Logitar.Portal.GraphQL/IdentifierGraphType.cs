using GraphQL.Types;
using Logitar.Portal.Contracts;
using Logitar.Portal.GraphQL.Actors;

namespace Logitar.Portal.GraphQL;

internal class IdentifierGraphType : ObjectGraphType<Identifier>
{
  public IdentifierGraphType()
  {
    Name = nameof(Identifier);
    Description = "Represents an unique identifier of an entity in the identity system.";

    Field(x => x.Id)
      .Description("The unique identifier of the identifier.");

    Field(x => x.Key)
      .Description("The key of the identifier.");
    Field(x => x.Value)
      .Description("The value of the identifier.");

    Field(x => x.CreatedBy, type: typeof(NonNullGraphType<ActorGraphType>))
      .Description("The actor who created the identifier.");
    Field(x => x.CreatedOn)
      .Description("The date and time when the identifier was created.");

    Field(x => x.UpdatedBy, type: typeof(NonNullGraphType<ActorGraphType>))
      .Description("The actor who updated the identifier lastly.");
    Field(x => x.UpdatedOn)
      .Description("The date and time when the identifier was updated lastly.");

    Field(x => x.Version)
      .Description("The version of the identifier.");
  }
}
