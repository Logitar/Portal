namespace Logitar.Portal.Client.Settings;

internal record SendGridSettings
{
  public string ApiKey { get; init; } = string.Empty;
}
