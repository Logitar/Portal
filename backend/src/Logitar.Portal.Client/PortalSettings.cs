namespace Logitar.Portal.Client;

public record PortalSettings : IPortalSettings
{
  public const string SectionKey = "Portal";

  public string? ApiKey { get; set; }
  public BasicCredentials? Basic { get; set; }

  public string? BaseUrl { get; set; }

  public string? Realm { get; set; }
}
