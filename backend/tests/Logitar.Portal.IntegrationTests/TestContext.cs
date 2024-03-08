using Logitar.Portal.Application;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal;

internal record TestContext
{
  public Realm? Realm { get; set; }
  public ApiKey? ApiKey { get; set; }
  public User? User { get; set; }
  public Session? Session { get; set; }

  public ActivityContext ToActivityContext(Configuration configuration) => new(configuration, Realm, ApiKey, User, Session);
}
