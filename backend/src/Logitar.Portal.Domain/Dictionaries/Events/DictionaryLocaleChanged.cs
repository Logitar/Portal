using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Portal.Domain.Dictionaries.Events;

public record DictionaryLocaleChanged(LocaleUnit Locale) : DomainEvent, INotification;
