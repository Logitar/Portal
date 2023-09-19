namespace Logitar.Portal.Settings;

internal record SendGridSettings
{
  public string ApiKey { get; init; } = string.Empty;
}
