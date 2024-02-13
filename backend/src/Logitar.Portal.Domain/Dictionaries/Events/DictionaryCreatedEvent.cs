using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Portal.Domain.Dictionaries.Events;

public record DictionaryCreatedEvent : DomainEvent, INotification
{
  public TenantId? TenantId { get; }

  public LocaleUnit Locale { get; }

  public DictionaryCreatedEvent(ActorId actorId, LocaleUnit locale, TenantId? tenantId)
  {
    ActorId = actorId;
    Locale = locale;
    TenantId = tenantId;
  }
}
