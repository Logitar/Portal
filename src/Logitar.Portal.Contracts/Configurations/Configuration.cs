using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Contracts.Configurations;

public record Configuration : Aggregate
{
  public Locale DefaultLocale { get; set; } = new();
  public string Secret { get; set; } = string.Empty;

  public UniqueNameSettings UniqueNameSettings { get; set; } = new();
  public PasswordSettings PasswordSettings { get; set; } = new();

  public LoggingSettings LoggingSettings { get; set; } = new();
}
