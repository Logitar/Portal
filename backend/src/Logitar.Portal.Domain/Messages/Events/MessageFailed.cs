using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events;

public record MessageFailed(IReadOnlyDictionary<string, string> ResultData) : DomainEvent, INotification;
