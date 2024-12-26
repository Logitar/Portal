using Logitar.EventSourcing;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Domain.Realms.Events;

public record RealmCreated(
  UniqueSlugUnit UniqueSlug,
  JwtSecretUnit Secret,
  ReadOnlyUniqueNameSettings UniqueNameSettings,
  ReadOnlyPasswordSettings PasswordSettings,
  bool RequireUniqueEmail) : DomainEvent, INotification;
