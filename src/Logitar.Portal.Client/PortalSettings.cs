namespace Logitar.Portal.Client;

public record PortalSettings
{
  public string BaseUrl { get; set; } = string.Empty;
  public Credentials? BasicAuthentication { get; set; }
}
