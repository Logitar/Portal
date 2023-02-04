using Logitar.Portal.Contracts;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events
{
  public record MessageFailedEvent : DomainEvent, INotification
  {
    public IEnumerable<Error> Errors { get; init; } = Enumerable.Empty<Error>();
  }
}
