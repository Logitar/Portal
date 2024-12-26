using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Activities;

public interface IContextParameters
{
  RealmModel? Realm { get; }
  ApiKeyModel? ApiKey { get; }
  UserModel? User { get; }
  SessionModel? Session { get; }

  string? RealmOverride { get; }
  string? ImpersonifiedUser { get; }
}
