using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderUpdated : DomainEvent, INotification
{
  public Email? Email { get; set; }
  public Phone? Phone { get; set; }
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  [JsonIgnore]
  public bool HasChanges => Email != null || Phone != null || DisplayName != null || Description != null;
}
