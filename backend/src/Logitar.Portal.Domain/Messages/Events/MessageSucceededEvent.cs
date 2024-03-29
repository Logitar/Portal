using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events;

public record MessageSucceededEvent(IReadOnlyDictionary<string, string> ResultData) : DomainEvent, INotification;
