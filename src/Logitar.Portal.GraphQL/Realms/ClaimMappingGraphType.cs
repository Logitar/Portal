using GraphQL.Types;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.GraphQL.Realms;

internal class ClaimMappingGraphType : ObjectGraphType<ClaimMapping>
{
  public ClaimMappingGraphType()
  {
    Name = nameof(ClaimMapping);
    Description = "Represents a mapping from a custom attribute to a claim.";

    Field(x => x.Key)
      .Description("The unique key of the custom attribute to which the claim is mapped to.");
    Field(x => x.Name)
      .Description("The name of the mapped claim.");
    Field(x => x.Type)
      .Description("The type of the mapped claim's value.");
  }
}
