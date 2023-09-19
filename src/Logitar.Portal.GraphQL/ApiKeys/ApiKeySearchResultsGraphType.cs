using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.GraphQL.ApiKeys;

internal class ApiKeySearchResultsGraphType : SearchResultsGraphType<ApiKeyGraphType, ApiKey>
{
  public ApiKeySearchResultsGraphType() : base("ApiKeySearchResults", "Represents the results of an API key search.")
  {
  }
}
