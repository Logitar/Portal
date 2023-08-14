using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application.Configurations;

public record Configuration : Aggregate
{
  public string DefaultLocale { get; set; } = string.Empty;
  public string Secret { get; set; } = string.Empty;

  public UniqueNameSettings UniqueNameSettings { get; set; } = new();
  public UniqueNameSettings PasswordSettings { get; set; } = new();

  public LoggingSettings LoggingSettings { get; set; } = new();
}
