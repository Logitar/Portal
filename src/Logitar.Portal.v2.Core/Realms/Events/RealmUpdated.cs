using MediatR;

namespace Logitar.Portal.v2.Core.Realms.Events;

public record RealmUpdated : RealmSaved, INotification;
