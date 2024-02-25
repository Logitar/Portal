using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Contracts.Configurations;

public class Configuration : Aggregate
{
  public string? DefaultLocale { get; set; } // TODO(fpion): Locale
  public string Secret { get; set; }

  public UniqueNameSettings UniqueNameSettings { get; set; } = new();
  public PasswordSettings PasswordSettings { get; set; } = new();
  public bool RequireUniqueEmail { get; set; }

  public LoggingSettings LoggingSettings { get; set; } = new();

  public Configuration() : this(string.Empty)
  {
  }

  public Configuration(string secret)
  {
    Secret = secret;
  }
}
