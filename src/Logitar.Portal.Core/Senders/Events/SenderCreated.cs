using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Core.Senders.Events;

public record SenderCreated : SenderSaved, INotification
{
  public AggregateId? RealmId { get; init; }

  public ProviderType Provider { get; init; }
}
