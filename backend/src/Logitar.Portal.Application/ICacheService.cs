using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application
{
  public interface ICacheService
  {
    Configuration? Configuration { get; set; }

    ApiKeyModel? GetApiKey(AggregateId id);
    void RemoveApiKey(AggregateId id);
    void SetApiKey(ApiKeyModel apiKey);
  }
}
