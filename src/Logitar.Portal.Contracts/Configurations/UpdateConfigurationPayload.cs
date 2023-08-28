using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Contracts.Configurations;

public record UpdateConfigurationPayload
{
  public string? DefaultLocale { get; set; }
  public string? Secret { get; set; }

  public UniqueNameSettings? UniqueNameSettings { get; set; }
  public PasswordSettings? PasswordSettings { get; set; }

  public LoggingSettings? LoggingSettings { get; set; }
}
