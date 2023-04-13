using MediatR;

namespace Logitar.Portal.Core.Realms.Events;

public record RealmUpdated : RealmSaved, INotification;
