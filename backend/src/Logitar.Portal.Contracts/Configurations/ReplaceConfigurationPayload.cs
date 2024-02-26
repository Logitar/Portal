using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Contracts.Configurations;

public record ReplaceConfigurationPayload
{
  public string? DefaultLocale { get; set; }
  public string Secret { get; set; }

  public UniqueNameSettings UniqueNameSettings { get; set; } = new();
  public PasswordSettings PasswordSettings { get; set; } = new();
  public bool RequireUniqueEmail { get; set; }

  public LoggingSettings LoggingSettings { get; set; } = new();

  public ReplaceConfigurationPayload() : this(string.Empty)
  {
  }

  public ReplaceConfigurationPayload(string secret)
  {
    Secret = secret;
  }
}
