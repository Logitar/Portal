using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal;

internal record TestContext
{
  public RealmModel? Realm { get; set; }
  public ApiKey? ApiKey { get; set; }
  public User? User { get; set; }
  public Session? Session { get; set; }
}
