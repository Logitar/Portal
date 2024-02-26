namespace Logitar.Portal.Application.Messages.Settings;

internal record EmailSettings
{
  public string Address { get; set; }
  public string? DisplayName { get; set; }

  public EmailSettings() : this(string.Empty)
  {
  }

  public EmailSettings(string address, string? displayName = null)
  {
    Address = address;
    DisplayName = displayName;
  }
}
