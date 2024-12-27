namespace Logitar.Portal.Worker.Settings;

internal record CronSettings
{
  public const string SectionKey = "Cron";

  public TimeSpan PurgeTokenBlacklist { get; set; }
}
