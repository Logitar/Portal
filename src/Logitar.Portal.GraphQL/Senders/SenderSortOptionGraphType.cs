using GraphQL.Types;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.GraphQL.Senders;

internal class SenderSortOptionGraphType : SortOptionInputGraphType<SenderSortOption>
{
  public SenderSortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<SenderSortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
