namespace Logitar.Portal.Core.Configurations.Payloads
{
  public record UserPayload
  {
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;

    public string Email { get; init; } = null!;

    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;

    public string Locale { get; init; } = null!;
  }
}
