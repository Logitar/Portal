using Logitar.Portal.Core.Templates.Events;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;

internal class TemplateEntity : AggregateEntity
{
  public TemplateEntity(TemplateCreated e, RealmEntity realm, ActorEntity actor) : base(e, actor)
  {
    Realm = realm;
    RealmId = realm.RealmId;

    UniqueName = e.UniqueName;

    Apply(e);
  }

  private TemplateEntity() : base()
  {
  }

  public int TemplateId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int RealmId { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => UniqueName.ToUpper();
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string Subject { get; private set; } = string.Empty;
  public string ContentType { get; private set; } = string.Empty;
  public string Contents { get; private set; } = string.Empty;

  public RealmEntity? PasswordRecoveryRealm { get; private set; }

  public void Update(TemplateUpdated e, ActorEntity actor)
  {
    base.Update(e, actor);

    Apply(e);
  }

  private void Apply(TemplateSaved e)
  {
    Subject = e.Subject;
    ContentType = e.ContentType;
    Contents = e.Contents;

    DisplayName = e.DisplayName;
    Description = e.Description;
  }
}
