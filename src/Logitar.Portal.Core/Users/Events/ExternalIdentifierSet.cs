using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Users.Events;

public record ExternalIdentifierSet : DomainEvent, INotification
{
  public string Key { get; init; } = string.Empty;
  public string? Value { get; init; }
}
