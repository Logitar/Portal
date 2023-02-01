namespace Logitar.Portal.Contracts.Accounts.Payloads
{
  public class RenewSessionPayload
  {
    public string RenewToken { get; set; } = string.Empty;

    public string? IpAddress { get; set; }
    public string? AdditionalInformation { get; set; }
  }
}
