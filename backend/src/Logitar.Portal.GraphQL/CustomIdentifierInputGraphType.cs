using GraphQL.Types;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.GraphQL;

internal class CustomIdentifierInputGraphType : InputObjectGraphType<CustomIdentifier>
{
  public CustomIdentifierInputGraphType()
  {
    Name = $"{nameof(CustomIdentifier)}Input";
    Description = "Represents an user-defined identifier of a resource.";

    Field(x => x.Key)
      .Description("The unique key of the custom identifier.");
    Field(x => x.Value)
      .Description("The value of the custom identifier.");
  }
}
