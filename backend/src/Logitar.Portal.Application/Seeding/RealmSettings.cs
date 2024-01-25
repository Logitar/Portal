namespace Logitar.Portal.Application.Seeding;

internal record RealmSettings
{
  public string DisplayName { get; set; } = "Portal";
  public string Description { get; set; } = "This is the platform administration realm.";
}
