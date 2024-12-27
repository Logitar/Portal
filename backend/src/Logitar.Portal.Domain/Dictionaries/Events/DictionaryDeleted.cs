using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Dictionaries.Events;

public record DictionaryDeleted : DomainEvent, IDeleteEvent, INotification;
