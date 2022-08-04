namespace Logitar.Portal.Core.Users.Payloads
{
  public class CreateUserSecurePayload : SaveUserSecurePayload
  {
    public string? Realm { get; set; }

    public string Username { get; set; } = null!;

    public bool ConfirmEmail { get; set; }
    public bool ConfirmPhoneNumber { get; set; }
  }
}
