namespace Portal.Core.Users.Payloads
{
  public class CreateUserSecurePayload : CreateUserPayloadBase
  {
    public string PasswordHash { get; set; } = null!;
  }
}
