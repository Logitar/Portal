namespace Logitar.Portal.Core2.Accounts.Payloads
{
  public class SignInPayload
  {
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool Remember { get; set; }

    public string? IpAddress { get; set; }
    public string? AdditionalInformation { get; set; }
  }
}
