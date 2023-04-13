using Logitar.EventSourcing;
using Logitar.Portal.Core.Users.Contact;
using MediatR;

namespace Logitar.Portal.Core.Users.Events;

public record PhoneChanged : DomainEvent, INotification
{
  public ReadOnlyPhone? Phone { get; init; }
  public VerificationAction? VerificationAction { get; init; }
}
