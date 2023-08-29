using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Domain.Passwords;
using MediatR;

namespace Logitar.Portal.Domain.Users.Events;

public record UserUpdatedEvent : DomainEvent, INotification
{
  public Password? Password { get; set; }

  public Modification<EmailAddress>? Email { get; set; }

  public Modification<string>? FirstName { get; set; }
  public Modification<string>? MiddleName { get; set; }
  public Modification<string>? LastName { get; set; }
  public Modification<string>? FullName { get; set; }
  public Modification<string>? Nickname { get; set; }

  public Modification<DateTime?>? Birthdate { get; set; }
  public Modification<Gender>? Gender { get; set; }
  public Modification<Locale>? Locale { get; set; }
}
