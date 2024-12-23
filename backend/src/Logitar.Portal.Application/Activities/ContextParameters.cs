using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Activities;

public record ContextParameters : IContextParameters
{
  public RealmModel? Realm { get; set; }
  public ApiKeyModel? ApiKey { get; set; }
  public UserModel? User { get; set; }
  public SessionModel? Session { get; set; }

  public string? RealmOverride { get; set; }
  public string? ImpersonifiedUser { get; set; }
}
