using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Portal.Domain.Dictionaries.Events;

public record DictionaryCreated(Locale Locale) : DomainEvent, INotification;
