namespace Logitar.Portal.Contracts.Configurations;

public record Configuration : Aggregate
{
  public string DefaultLocale { get; set; } = string.Empty;
  public string Secret { get; set; } = string.Empty;

  public UniqueNameSettings UniqueNameSettings { get; set; } = new();
  public PasswordSettings PasswordSettings { get; set; } = new();

  public LoggingSettings LoggingSettings { get; set; } = new();
}
