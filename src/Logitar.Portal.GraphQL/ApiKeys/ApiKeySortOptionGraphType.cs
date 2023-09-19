using GraphQL.Types;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.GraphQL.ApiKeys;

internal class ApiKeySortOptionGraphType : SortOptionInputGraphType<ApiKeySortOption>
{
  public ApiKeySortOptionGraphType() : base()
  {
    Field(x => x.Field, type: typeof(NonNullGraphType<ApiKeySortGraphType>))
      .Description("The field on which to apply the sort.");
  }
}
