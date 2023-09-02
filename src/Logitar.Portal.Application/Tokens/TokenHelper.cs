using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Tokens;

internal static class TokenHelper
{
  public static string? GetAudience(string? format, RealmAggregate? realm, Uri? baseUrl)
  {
    string? audience = Format(format, realm);
    if (audience != null)
    {
      return audience;
    }
    else if (realm != null)
    {
      return realm.Url?.ToString() ?? realm.UniqueSlug;
    }

    return baseUrl?.ToString();
  }

  public static string? GetIssuer(string? format, RealmAggregate? realm, Uri? baseUrl)
  {
    string? issuer = Format(format, realm);
    if (issuer != null)
    {
      return issuer;
    }
    else if (realm != null)
    {
      return baseUrl == null
        ? realm.Url?.ToString() ?? realm.UniqueSlug
        : string.Join('/', baseUrl.ToString().TrimEnd('/'), "realms", realm.UniqueSlug);
    }

    return baseUrl?.ToString();
  }

  private static string? Format(string? format, RealmAggregate? realm) => format?
    .Replace("{UniqueSlug}", realm?.UniqueSlug)
    .Replace("{Url}", realm?.Url?.ToString())
    .CleanTrim();
}
