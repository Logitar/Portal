namespace Logitar.Portal.Application.Seeding;

internal record SeedingSettings
{
  public RealmSettings Realm { get; set; } = new();
  public UserSettings User { get; set; } = new();
}
