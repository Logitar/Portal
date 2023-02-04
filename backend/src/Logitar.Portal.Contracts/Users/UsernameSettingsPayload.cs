namespace Logitar.Portal.Contracts.Users
{
  public record UsernameSettingsPayload
  {
    public string? AllowedCharacters { get; set; }
  }
}
