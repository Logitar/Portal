using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Domain.Passwords;

namespace Logitar.Portal.Domain.Users.Events;

public record UserUpdatedEvent : DomainEvent
{
  public Password? Password { get; set; }

  public Modification<EmailAddress>? Email { get; set; }

  public Modification<string>? FirstName { get; set; }
  public Modification<string>? LastName { get; set; }
  public Modification<string>? FullName { get; set; }
}
