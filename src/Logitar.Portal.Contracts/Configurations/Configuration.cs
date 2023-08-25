using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Contracts.Configurations;

public record Configuration
{
  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }

  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public long Version { get; set; }

  public string DefaultLocale { get; set; } = string.Empty;
  public string Secret { get; set; } = string.Empty;

  public UniqueNameSettings UniqueNameSettings { get; set; } = new();
  public PasswordSettings PasswordSettings { get; set; } = new();

  public LoggingSettings LoggingSettings { get; set; } = new();
}
