using GraphQL.Types;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.GraphQL.Messages;

internal class VariableGraphType : ObjectGraphType<Variable>
{
  public VariableGraphType()
  {
    Name = nameof(Variable);
    Description = "Represents a message variable used to inject a value in a compiled body.";

    Field(x => x.Key)
      .Description("The unique key of the variable.");
    Field(x => x.Value)
      .Description("The value of the variable.");
  }
}
