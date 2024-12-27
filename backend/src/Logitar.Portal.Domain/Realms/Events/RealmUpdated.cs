using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Domain.Realms.Events;

public record RealmUpdated : DomainEvent, INotification
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public Change<Locale>? DefaultLocale { get; set; }
  public JwtSecret? Secret { get; set; }
  public Change<Url>? Url { get; set; }

  public ReadOnlyUniqueNameSettings? UniqueNameSettings { get; set; }
  public ReadOnlyPasswordSettings? PasswordSettings { get; set; }
  public bool? RequireUniqueEmail { get; set; }

  public Dictionary<Identifier, string?> CustomAttributes { get; init; } = [];

  [JsonIgnore]
  public bool HasChanges => DisplayName != null || Description != null
    || DefaultLocale != null || Secret != null || Url != null
    || UniqueNameSettings != null || PasswordSettings != null || RequireUniqueEmail.HasValue
    || CustomAttributes.Count > 0;
}
