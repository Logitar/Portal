namespace Logitar.Portal.Client;

public record PortalSettings : IPortalSettings
{
  public string? ApiKey { get; set; }
  public BasicCredentials? Basic { get; set; }

  public string? BaseUrl { get; set; }

  public string? Realm { get; set; }
}
