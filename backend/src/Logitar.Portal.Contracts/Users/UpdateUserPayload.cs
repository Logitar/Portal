namespace Logitar.Portal.Contracts.Users;

public record UpdateUserPayload
{
  public string? UniqueName { get; set; }
  public ChangePasswordPayload? Password { get; set; }
  public bool? IsDisabled { get; set; }

  public Modification<AddressPayload>? Address { get; set; }
  public Modification<EmailPayload>? Email { get; set; }
  public Modification<PhonePayload>? Phone { get; set; }
}
