using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Claims
{
  internal static class ClaimHelper
  {
    public static string GetAudience(Realm? realm, IUserContext userContext) => (realm?.Url ?? realm?.Alias ?? userContext.BaseUrl).ToLower();
    public static string GetIssuer(Realm? realm, IUserContext userContext) => (realm == null ? userContext.BaseUrl : $"{userContext.BaseUrl}/realms/{realm.Alias}").ToLower();
  }
}
