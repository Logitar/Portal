using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.v2.Core.Senders.Events;

public record SenderCreated : SenderSaved, INotification
{
  public AggregateId RealmId { get; init; }

  public ProviderType Provider { get; init; }
}
