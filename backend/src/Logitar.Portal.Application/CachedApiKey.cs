using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.ApiKeys;

namespace Logitar.Portal.Application
{
  public record CachedApiKey(ApiKey Aggregate, ApiKeyModel Model)
  {
    public readonly AggregateId Id = Aggregate.Id;
  };
}
