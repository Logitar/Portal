using Logitar.Identity.Contracts;
using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Contracts.Configurations;

public record UpdateConfigurationPayload
{
  public Modification<string>? DefaultLocale { get; set; }
  public string? Secret { get; set; }

  public UniqueNameSettings? UniqueNameSettings { get; set; }
  public PasswordSettings? PasswordSettings { get; set; }
  public bool? RequireUniqueEmail { get; set; }

  public LoggingSettings? LoggingSettings { get; set; }
}
