namespace Logitar.Portal.Contracts.Configurations;

public record InitializeConfigurationPayload
{
  public string Locale { get; set; } = string.Empty;
  public UserPayload User { get; set; } = new();
}
