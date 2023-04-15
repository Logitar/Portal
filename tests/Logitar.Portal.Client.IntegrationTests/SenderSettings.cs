namespace Logitar.Portal.Client;

internal record SenderSettings
{
  public string EmailAddress { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
}
