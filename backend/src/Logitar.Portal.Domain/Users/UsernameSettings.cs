namespace Logitar.Portal.Domain.Users
{
  public record UsernameSettings
  {
    public string? AllowedCharacters { get; init; }
  }
}
