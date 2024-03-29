namespace Logitar.Portal.Application.Messages.Settings;

internal record RecipientSettings
{
  public string Address { get; set; }
  public string? DisplayName { get; set; }

  public string PhoneNumber { get; set; }

  public RecipientSettings() : this(string.Empty, string.Empty)
  {
  }

  public RecipientSettings(string address, string phoneNumber, string? displayName = null)
  {
    Address = address;
    PhoneNumber = phoneNumber;
    DisplayName = displayName;
  }
}
