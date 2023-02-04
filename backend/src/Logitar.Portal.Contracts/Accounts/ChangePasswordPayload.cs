namespace Logitar.Portal.Contracts.Accounts
{
  public record ChangePasswordPayload
  {
    public string Current { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
  }
}
