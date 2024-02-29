using Logitar.Net.Http;
using Logitar.Portal.Contracts.Constants;

namespace Logitar.Portal.Client;

internal static class PortalSettingsExtensions
{
  public static HttpApiSettings ToHttpApiSettings(this IPortalSettings settings)
  {
    HttpApiSettings apiSettings = new();

    if (!string.IsNullOrWhiteSpace(settings.ApiKey))
    {
      apiSettings.Headers.Add(new HttpHeader(Headers.ApiKey, settings.ApiKey.Trim()));
    }
    else if (settings.Basic != null)
    {
      apiSettings.Authorization = HttpAuthorization.Basic(settings.Basic.Username.Trim(), settings.Basic.Password.Trim());
    }

    if (!string.IsNullOrWhiteSpace(settings.BaseUrl))
    {
      apiSettings.BaseUri = new Uri(settings.BaseUrl.Trim(), UriKind.Absolute);
    }

    if (!string.IsNullOrWhiteSpace(settings.Realm))
    {
      apiSettings.Headers.Add(new HttpHeader(Headers.Realm, settings.Realm.Trim()));
    }

    return apiSettings;
  }
}
