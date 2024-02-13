using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Portal.Domain.Dictionaries.Events;

public record DictionaryLocaleChangedEvent : DomainEvent, INotification
{
  public LocaleUnit Locale { get; }

  public DictionaryLocaleChangedEvent(ActorId actorId, LocaleUnit locale)
  {
    ActorId = actorId;
    Locale = locale;
  }
}
