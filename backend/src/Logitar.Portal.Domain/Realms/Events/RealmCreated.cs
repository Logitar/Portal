using Logitar.EventSourcing;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Domain.Realms.Events;

public record RealmCreated(
  Slug UniqueSlug,
  JwtSecret Secret,
  ReadOnlyUniqueNameSettings UniqueNameSettings,
  ReadOnlyPasswordSettings PasswordSettings,
  bool RequireUniqueEmail) : DomainEvent, INotification;
