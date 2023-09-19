using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Dictionaries.Events;

public record DictionaryCreatedEvent : DomainEvent, INotification
{
  public DictionaryCreatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public string? TenantId { get; init; }

  public ReadOnlyLocale Locale { get; init; } = ReadOnlyLocale.Default;
}
