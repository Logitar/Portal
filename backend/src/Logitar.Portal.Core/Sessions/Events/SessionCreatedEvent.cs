using MediatR;

namespace Logitar.Portal.Core.Sessions.Events
{
  internal class SessionCreatedEvent : DomainEvent, INotification
  {
    public string? KeyHash { get; init; }

    public string? IpAddress { get; init; }
    public string? AdditionalInformation { get; init; }
  }
}
