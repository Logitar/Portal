using MediatR;

namespace Logitar.Portal.Domain.Senders.Events
{
  public record SenderSetDefaultEvent : DomainEvent, INotification
  {
    public bool IsDefault { get; init; }
  }
}
