using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events;

public record MessageDeleted : DomainEvent, IDeleteEvent, INotification;
