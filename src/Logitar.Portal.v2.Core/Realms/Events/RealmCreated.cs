using MediatR;

namespace Logitar.Portal.v2.Core.Realms.Events;

public record RealmCreated : RealmSaved, INotification
{
  public string UniqueName { get; init; } = string.Empty;
}
