using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application
{
  public interface ICacheService
  {
    Configuration? Configuration { get; set; }

    CachedApiKey? GetApiKey(AggregateId id);
    void RemoveApiKey(AggregateId id);
    void SetApiKey(CachedApiKey apiKey);
  }
}
