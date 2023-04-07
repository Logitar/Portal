using Logitar.EventSourcing;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.v2.Core.Users.Events;

public record UserCreated : DomainEvent, INotification
{
  public AggregateId? RealmId { get; init; }

  public string Username { get; init; } = string.Empty;

  public string? FirstName { get; init; }
  public string? MiddleName { get; init; }
  public string? LastName { get; init; }
  public string? FullName { get; init; }
  public string? Nickname { get; init; }

  public DateTime? Birthdate { get; init; }
  public Gender? Gender { get; init; }

  public CultureInfo? Locale { get; init; }
  public TimeZoneEntry? TimeZone { get; init; }

  public Uri? Picture { get; init; }
  public Uri? Profile { get; init; }
  public Uri? Website { get; init; }

  public Dictionary<string, string> CustomAttributes { get; init; } = new();
}
