namespace Logitar.Portal.Core.Accounts.Payloads
{
  public class RenewSessionPayload
  {
    public string RenewToken { get; set; } = null!;

    public string? IpAddress { get; set; }
    public string? AdditionalInformation { get; set; }
  }
}
