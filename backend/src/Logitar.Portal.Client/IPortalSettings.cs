namespace Logitar.Portal.Client;

public interface IPortalSettings
{
  string? ApiKey { get; }
  BasicCredentials? Basic { get; }

  string? BaseUrl { get; }

  string? Realm { get; }
}
