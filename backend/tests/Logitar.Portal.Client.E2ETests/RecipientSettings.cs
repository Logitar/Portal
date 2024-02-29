namespace Logitar.Portal;

internal record RecipientSettings
{
  public string Address { get; set; }
  public string? DisplayName { get; set; }

  public RecipientSettings() : this(string.Empty)
  {
  }

  public RecipientSettings(string address, string? displayName = null)
  {
    Address = address;
    DisplayName = displayName;
  }
}
