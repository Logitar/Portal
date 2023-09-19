using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Domain.Realms.Events;

public record RealmUpdatedEvent : DomainEvent, INotification
{
  public string? UniqueSlug { get; set; }
  public Modification<string>? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public Modification<ReadOnlyLocale>? DefaultLocale { get; set; }
  public JwtSecret? Secret { get; set; }
  public Modification<Uri>? Url { get; set; }

  public bool? RequireUniqueEmail { get; set; }
  public bool? RequireConfirmedAccount { get; set; }

  public ReadOnlyUniqueNameSettings? UniqueNameSettings { get; set; }
  public ReadOnlyPasswordSettings? PasswordSettings { get; set; }

  public Dictionary<string, ReadOnlyClaimMapping?> ClaimMappings { get; init; } = new();

  public Dictionary<string, string?> CustomAttributes { get; init; } = new();

  public Modification<AggregateId?>? PasswordRecoverySenderId { get; set; }
  public Modification<AggregateId?>? PasswordRecoveryTemplateId { get; set; }
}
