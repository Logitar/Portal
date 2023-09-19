using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderUpdatedEvent : DomainEvent, INotification
{
  public bool? IsDefault { get; set; }

  public string? EmailAddress { get; set; }
  public Modification<string>? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public Dictionary<string, string?> Settings { get; init; } = new();
}
