using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events;

public record MessageFailedEvent(IReadOnlyDictionary<string, string> ResultData) : DomainEvent, INotification;
