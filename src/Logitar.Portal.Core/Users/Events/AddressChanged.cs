using Logitar.EventSourcing;
using Logitar.Portal.Core.Users.Contact;
using MediatR;

namespace Logitar.Portal.Core.Users.Events;

public record AddressChanged : DomainEvent, INotification
{
  public ReadOnlyAddress? Address { get; init; }
  public VerificationAction? VerificationAction { get; init; }
}
