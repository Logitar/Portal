using GraphQL.Types;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.GraphQL;
internal class CustomAttributeGraphType : ObjectGraphType<CustomAttribute>
{
  public CustomAttributeGraphType()
  {
    Name = nameof(CustomAttribute);
    Description = "Represents an user-defined attribute of a resource.";

    Field(x => x.Key)
      .Description("The unique key of the custom attribute.");
    Field(x => x.Value)
      .Description("The value of the custom attribute.");
  }
}
