namespace Logitar.Portal.Application.Messages;

internal record SenderSettings
{
  public string SendGridApiKey { get; set; }

  public string Address { get; set; }
  public string? DisplayName { get; set; }

  public SenderSettings() : this(string.Empty, string.Empty)
  {
  }

  public SenderSettings(string sendGridApiKey, string address, string? displayName = null)
  {
    SendGridApiKey = sendGridApiKey;
    Address = address;
    DisplayName = displayName;
  }
}
