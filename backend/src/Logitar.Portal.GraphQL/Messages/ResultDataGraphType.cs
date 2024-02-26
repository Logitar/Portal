using GraphQL.Types;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.GraphQL.Messages;

internal class ResultDataGraphType : ObjectGraphType<ResultData>
{
  public ResultDataGraphType()
  {
    Name = nameof(ResultData);
    Description = "Represents a property describing the result of sending a message.";

    Field(x => x.Key)
      .Description("The unique key of the property.");
    Field(x => x.Value)
      .Description("The value of the property.");
  }
}
