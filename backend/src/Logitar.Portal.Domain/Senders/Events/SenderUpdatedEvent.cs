using Logitar.EventSourcing;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderUpdatedEvent : DomainEvent, INotification
{
  public EmailUnit? Email { get; set; }
  public PhoneUnit? Phone { get; set; }
  public Modification<DisplayNameUnit>? DisplayName { get; set; }
  public Modification<DescriptionUnit>? Description { get; set; }

  public bool HasChanges => Email != null || Phone != null || DisplayName != null || Description != null;
}
