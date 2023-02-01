using MediatR;

namespace Logitar.Portal.Domain.Sessions.Events
{
  public record SessionCreatedEvent : DomainEvent, INotification
  {
    public string? KeyHash { get; init; }

    public string? IpAddress { get; init; }
    public string? AdditionalInformation { get; init; }
  }
}
