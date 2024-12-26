using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Realms.Events;

public record RealmUniqueSlugChanged(UniqueSlugUnit UniqueSlug) : DomainEvent, INotification;
