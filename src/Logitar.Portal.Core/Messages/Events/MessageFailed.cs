using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Errors;
using MediatR;

namespace Logitar.Portal.Core.Messages.Events;

public record MessageFailed : DomainEvent, INotification
{
  public IEnumerable<Error> Errors { get; init; } = Enumerable.Empty<Error>();
}
