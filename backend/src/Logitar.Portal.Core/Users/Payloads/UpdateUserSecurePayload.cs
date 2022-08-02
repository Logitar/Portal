namespace Logitar.Portal.Core.Users.Payloads
{
  public class UpdateUserSecurePayload : SaveUserPayload
  {
    public string? PasswordHash { get; set; }
  }
}
