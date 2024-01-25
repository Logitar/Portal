namespace Logitar.Portal.Application.Seeding;

internal record UserSettings
{
  public string UniqueName { get; set; } = "admin";
  public string Password { get; set; } = "P0rTa7!*";
}
