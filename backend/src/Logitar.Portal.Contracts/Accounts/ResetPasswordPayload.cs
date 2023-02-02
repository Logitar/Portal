namespace Logitar.Portal.Contracts.Accounts
{
  public record ResetPasswordPayload
  {
    public string Token { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
  }
}
