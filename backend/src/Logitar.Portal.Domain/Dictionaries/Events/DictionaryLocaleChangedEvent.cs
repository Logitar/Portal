using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Portal.Domain.Dictionaries.Events;

public record DictionaryLocaleChangedEvent(LocaleUnit Locale) : DomainEvent, INotification;
