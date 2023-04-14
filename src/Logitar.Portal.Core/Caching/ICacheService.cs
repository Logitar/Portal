using Logitar.Portal.Core.Realms;

namespace Logitar.Portal.Core.Caching;

public interface ICacheService
{
  RealmAggregate? PortalRealm { get; set; }
}
