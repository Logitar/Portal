using MediatR;

namespace Logitar.Portal.Core.Sessions.Events
{
  public class SessionRenewedEvent : DomainEvent, INotification
  {
    public string KeyHash { get; init; } = null!;

    public string? IpAddress { get; init; }
    public string? AdditionalInformation { get; init; }
  }
}
