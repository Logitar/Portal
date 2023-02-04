using MediatR;

namespace Logitar.Portal.Domain.Senders.Events
{
  public record SenderUpdatedEvent : DomainEvent, INotification
  {
    public string EmailAddress { get; init; } = string.Empty;
    public string? DisplayName { get; init; }

    public Dictionary<string, string?>? Settings { get; init; }
  }
}
