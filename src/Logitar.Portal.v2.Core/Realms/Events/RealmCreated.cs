namespace Logitar.Portal.v2.Core.Realms.Events;

internal record RealmCreated : RealmSaved
{
  public string UniqueName { get; init; } = string.Empty;
}
