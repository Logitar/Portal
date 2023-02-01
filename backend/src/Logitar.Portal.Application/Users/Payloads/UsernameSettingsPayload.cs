namespace Logitar.Portal.Application.Users.Payloads
{
  public record UsernameSettingsPayload
  {
    public string? AllowedCharacters { get; set; }
  }
}
