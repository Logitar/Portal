using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Realms.Events;

public record RealmUniqueSlugChangedEvent(UniqueSlugUnit UniqueSlug) : DomainEvent, INotification;
