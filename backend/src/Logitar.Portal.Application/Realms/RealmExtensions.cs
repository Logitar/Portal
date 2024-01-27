using Logitar.Identity.Domain.ApiKeys;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Application.Realms;

internal static class RealmExtensions
{
  public static void EnsureIsInRealm(this ApiKeyAggregate apiKey, IApplicationContext applicationContext)
  {
    apiKey.EnsureIsInRealm(applicationContext.Realm);
  }
  public static void EnsureIsInRealm(this ApiKeyAggregate apiKey, Realm? realm)
  {
    if (apiKey.TenantId?.Value != realm?.Id)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }
  }
}
