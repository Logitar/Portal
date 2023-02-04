namespace Logitar.Portal.Contracts.Accounts
{
  public record RenewSessionPayload
  {
    public string RenewToken { get; set; } = string.Empty;

    public string? IpAddress { get; set; }
    public string? AdditionalInformation { get; set; }
  }
}
