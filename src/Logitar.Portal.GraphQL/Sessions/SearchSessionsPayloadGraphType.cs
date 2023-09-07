using GraphQL.Types;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.GraphQL.Sessions;

internal class SearchSessionsPayloadGraphType : SearchPayloadInputGraphType<SearchSessionsPayload>
{
  public SearchSessionsPayloadGraphType() : base()
  {
    Field(x => x.Realm, nullable: true)
      .Description("The unique identifier or unique name of the realm in which to search.");
    Field(x => x.UserId, nullable: true)
      .Description("The unique identifier of the user to filter with.");

    Field(x => x.IsActive, nullable: true)
      .Description("When specified, will filter sessions which are active or not.");
    Field(x => x.IsPersistent, nullable: true)
      .Description("When specified, will filter sessions which are persistent or not.");

    Field(x => x.Sort, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<SessionSortOptionGraphType>>>))
      .DefaultValue(Enumerable.Empty<SessionSortOption>())
      .Description("The sort parameters of the search.");
  }
}
