using Logitar.Portal.Contracts.Sessions;
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

    SessionModel? GetSession(string id);
    void RemoveSession(AggregateId id);
    void RemoveSessions(IEnumerable<string> ids);
    void SetSession(SessionModel session);
  }
}
