using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.v2.Core.Users.Events;

public record UserCreated : UserSaved, INotification
{
  public AggregateId? RealmId { get; init; }

  public string Username { get; init; } = string.Empty;
}
