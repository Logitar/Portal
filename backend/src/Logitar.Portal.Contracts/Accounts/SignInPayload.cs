namespace Logitar.Portal.Contracts.Accounts
{
  public record SignInPayload
  {
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool Remember { get; set; }

    public string? IpAddress { get; set; }
    public string? AdditionalInformation { get; set; }
  }
}
