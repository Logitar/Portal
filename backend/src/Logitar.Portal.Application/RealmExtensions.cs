using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application;

public static class RealmExtensions
{
  public static TenantId GetTenantId(this RealmModel realm) => new(new RealmId(realm.Id).Value);
}
