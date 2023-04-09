using Logitar.Portal.v2.Core.Realms.Events;
using System.Text.Json;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;

internal class RealmEntity : AggregateEntity, ICustomAttributes
{
  public RealmEntity(RealmCreated e, ActorEntity actor) : base(e, actor)
  {
    UniqueName = e.UniqueName;

    Apply(e);
  }

  private RealmEntity() : base()
  {
  }

  public int RealmId { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => UniqueName.ToUpper();
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string? DefaultLocale { get; private set; }
  public string Secret { get; private set; } = string.Empty;
  public string? Url { get; private set; }

  public bool RequireConfirmedAccount { get; private set; }
  public bool RequireUniqueEmail { get; private set; }

  public string UsernameSettings { get; private set; } = string.Empty;
  public string PasswordSettings { get; private set; } = string.Empty;

  public string? ClaimMappings { get; private set; }
  public string? CustomAttributes { get; private set; }

  public List<ExternalIdentifierEntity> ExternalIdentifiers { get; private set; } = new();
  public List<UserEntity> Users { get; private set; } = new();

  public void Update(RealmUpdated e, ActorEntity actor)
  {
    base.Update(e, actor);

    Apply(e);
  }

  private void Apply(RealmSaved e)
  {
    DisplayName = e.DisplayName;
    Description = e.Description;

    DefaultLocale = e.DefaultLocale?.Name;
    Secret = e.Secret;
    Url = e.Url?.ToString();

    RequireConfirmedAccount = e.RequireConfirmedAccount;
    RequireUniqueEmail = e.RequireUniqueEmail;

    UsernameSettings = JsonSerializer.Serialize(e.UsernameSettings);
    PasswordSettings = JsonSerializer.Serialize(e.PasswordSettings);

    ClaimMappings = e.ClaimMappings.Any() ? JsonSerializer.Serialize(e.ClaimMappings) : null;
    CustomAttributes = e.CustomAttributes.Any() ? JsonSerializer.Serialize(e.CustomAttributes) : null;
  }
}
