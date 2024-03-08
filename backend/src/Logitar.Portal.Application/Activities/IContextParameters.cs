using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Activities;

public interface IContextParameters
{
  Realm? Realm { get; }
  ApiKey? ApiKey { get; }
  User? User { get; }
  Session? Session { get; }

  string? RealmOverride { get; }
  string? ImpersonifiedUser { get; }
}
