using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Domain.Passwords;
using MediatR;

namespace Logitar.Portal.Domain.Users.Events;

public record UserUpdatedEvent : DomainEvent, INotification
{
  public string? UniqueName { get; set; }
  public Password? Password { get; set; }

  public Modification<PostalAddress>? Address { get; set; }
  public Modification<EmailAddress>? Email { get; set; }
  public Modification<PhoneNumber>? Phone { get; set; }

  public Modification<string>? FirstName { get; set; }
  public Modification<string>? MiddleName { get; set; }
  public Modification<string>? LastName { get; set; }
  public Modification<string>? FullName { get; set; }
  public Modification<string>? Nickname { get; set; }

  public Modification<DateTime?>? Birthdate { get; set; }
  public Modification<Gender>? Gender { get; set; }
  public Modification<Locale>? Locale { get; set; }
  public Modification<TimeZoneEntry>? TimeZone { get; set; }

  public Modification<Uri>? Picture { get; set; }
  public Modification<Uri>? Profile { get; set; }
  public Modification<Uri>? Website { get; set; }

  public Dictionary<string, string?> CustomAttributes { get; init; } = new();

  public Dictionary<string, CollectionAction> Roles { get; init; } = new();
}
