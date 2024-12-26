using Logitar.EventSourcing;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Domain.Realms.Events;

public record RealmUpdated : DomainEvent, INotification
{
  public Modification<DisplayNameUnit>? DisplayName { get; set; }
  public Modification<DescriptionUnit>? Description { get; set; }

  public Modification<LocaleUnit>? DefaultLocale { get; set; }
  public JwtSecretUnit? Secret { get; set; }
  public Modification<UrlUnit>? Url { get; set; }

  public ReadOnlyUniqueNameSettings? UniqueNameSettings { get; set; }
  public ReadOnlyPasswordSettings? PasswordSettings { get; set; }
  public bool? RequireUniqueEmail { get; set; }

  public Dictionary<string, string?> CustomAttributes { get; init; } = [];

  [JsonIgnore]
  public bool HasChanges => DisplayName != null || Description != null
    || DefaultLocale != null || Secret != null || Url != null
    || UniqueNameSettings != null || PasswordSettings != null || RequireUniqueEmail.HasValue
    || CustomAttributes.Count > 0;
}
