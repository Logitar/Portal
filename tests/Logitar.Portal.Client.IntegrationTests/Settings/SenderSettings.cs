namespace Logitar.Portal.Client.Settings;

internal record SenderSettings
{
  public string Address { get; set; } = string.Empty;
  public string? DisplayName { get; init; }
}
