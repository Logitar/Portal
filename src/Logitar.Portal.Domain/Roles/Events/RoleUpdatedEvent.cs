using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using MediatR;

namespace Logitar.Portal.Domain.Roles.Events;

public record RoleUpdatedEvent : DomainEvent, INotification
{
  public string? UniqueName { get; set; }
  public Modification<string>? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public Dictionary<string, string?> CustomAttributes { get; init; } = new();
}
