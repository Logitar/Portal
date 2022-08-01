namespace Portal.Core.Users.Payloads
{
  public class UpdateUserPayload : SaveUserPayload
  {
    public string? Password { get; set; }
  }
}
