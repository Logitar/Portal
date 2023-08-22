using Logitar.EventSourcing;
using Logitar.Identity.Domain;
using Logitar.Portal.Contracts;
using MediatR;

namespace Logitar.Portal.Domain.Realms.Events;

public record RealmUpdatedEvent : DomainEvent, INotification
{
  public string? UniqueSlug { get; set; }
  public Modification<string>? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public Modification<Locale>? DefaultLocale { get; set; }
  public string? Secret { get; set; }
  public Modification<Uri>? Url { get; set; }

  public bool? RequireUniqueEmail { get; set; }
  public bool? RequireConfirmedAccount { get; set; }

  public ReadOnlyUniqueNameSettings? UniqueNameSettings { get; set; }
  public ReadOnlyPasswordSettings? PasswordSettings { get; set; }

  public Dictionary<string, ReadOnlyClaimMapping?> ClaimMappings { get; init; } = new();

  public Dictionary<string, string?> CustomAttributes { get; init; } = new();
}
