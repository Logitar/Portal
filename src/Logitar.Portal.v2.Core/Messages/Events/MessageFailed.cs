using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Errors;
using MediatR;

namespace Logitar.Portal.v2.Core.Messages.Events;

public record MessageFailed : DomainEvent, INotification
{
  public IEnumerable<Error> Errors { get; init; } = Enumerable.Empty<Error>();
}
