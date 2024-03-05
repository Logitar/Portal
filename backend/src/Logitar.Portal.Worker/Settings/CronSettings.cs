namespace Logitar.Portal.Worker.Settings;

internal record CronSettings
{
  public TimeSpan PurgeTokenBlacklist { get; set; }
}
