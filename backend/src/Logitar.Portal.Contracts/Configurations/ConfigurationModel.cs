using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Contracts.Configurations;

public class ConfigurationModel : AggregateModel
{
  public LocaleModel? DefaultLocale { get; set; }
  public string Secret { get; set; }

  public UniqueNameSettings UniqueNameSettings { get; set; } = new();
  public PasswordSettings PasswordSettings { get; set; } = new();
  public bool RequireUniqueEmail { get; set; }

  public LoggingSettings LoggingSettings { get; set; } = new();

  public ConfigurationModel() : this(string.Empty)
  {
  }

  public ConfigurationModel(string secret)
  {
    Secret = secret;
  }
}
