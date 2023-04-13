using MediatR;

namespace Logitar.Portal.Core.Realms.Events;

public record RealmCreated : RealmSaved, INotification
{
  public string UniqueName { get; init; } = string.Empty;
}
