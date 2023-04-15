namespace Logitar.Portal.Client;

internal record SendGridSettings
{
  public string ApiKey { get; set; } = string.Empty;
}
