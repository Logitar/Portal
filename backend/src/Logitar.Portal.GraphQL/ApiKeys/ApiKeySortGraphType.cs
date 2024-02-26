using GraphQL.Types;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.GraphQL.ApiKeys;

internal class ApiKeySortGraphType : EnumerationGraphType<ApiKeySort>
{
  public ApiKeySortGraphType()
  {
    Name = nameof(ApiKeySort);
    Description = "Represents the available API key fields for sorting.";

    Add(ApiKeySort.AuthenticatedOn, "The API keys will be sorted by their latest authentication date and time.");
    Add(ApiKeySort.DisplayName, "The API keys will be sorted by their display name.");
    Add(ApiKeySort.ExpiresOn, "The API keys will be sorted by expiration date and time.");
    Add(ApiKeySort.UpdatedOn, "The API keys will be sorted by their latest update date and time.");
  }

  private void Add(ApiKeySort value, string description) => Add(value.ToString(), value, description);
}
