namespace Logitar.Portal.Core.Users.Payloads
{
  public class CreateUserPayload : CreateUserPayloadBase
  {
    public string Password { get; set; } = null!;
  }
}
