using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using MediatR;

namespace Logitar.Portal.Domain.ApiKeys.Events;

public record ApiKeyUpdatedEvent : DomainEvent, INotification
{
  public string? Title { get; set; }
  public Modification<string>? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public Dictionary<string, string?> CustomAttributes { get; init; } = new();

  public Dictionary<string, CollectionAction> Roles { get; init; } = new();
}
