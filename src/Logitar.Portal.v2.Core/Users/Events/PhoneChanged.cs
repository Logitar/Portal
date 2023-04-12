using Logitar.EventSourcing;
using Logitar.Portal.v2.Core.Users.Contact;
using MediatR;

namespace Logitar.Portal.v2.Core.Users.Events;

public record PhoneChanged : DomainEvent, INotification
{
  public ReadOnlyPhone? Phone { get; init; }
  public VerificationAction? VerificationAction { get; init; }
}
