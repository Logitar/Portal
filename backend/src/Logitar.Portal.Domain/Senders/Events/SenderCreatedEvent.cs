using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events
{
  public record SenderCreatedEvent : DomainEvent, INotification
  {
    public AggregateId? RealmId { get; init; }

    public bool IsDefault { get; init; }

    public string EmailAddress { get; init; } = string.Empty;
    public string? DisplayName { get; init; }

    public ProviderType Provider { get; init; }
    public Dictionary<string, string?>? Settings { get; init; }
  }
}
