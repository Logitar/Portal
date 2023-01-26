namespace Logitar.Portal.Core2.Configurations.Payloads
{
  internal record UserPayload
  {
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;

    public string Email { get; init; } = null!;

    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;

    public string Locale { get; init; } = null!;
  }
}
