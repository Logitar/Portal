using Logitar.EventSourcing;
using Logitar.Portal.v2.Core.Users.Contact;
using MediatR;

namespace Logitar.Portal.v2.Core.Users.Events;

public record AddressChanged : DomainEvent, INotification
{
  public ReadOnlyAddress? Address { get; init; }
  public VerificationAction? VerificationAction { get; init; }
}
