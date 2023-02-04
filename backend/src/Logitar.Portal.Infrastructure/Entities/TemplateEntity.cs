using Logitar.Portal.Domain.Templates.Events;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class TemplateEntity : AggregateEntity
  {
    public TemplateEntity(TemplateCreatedEvent @event, Actor actor, RealmEntity? realm = null) : base(@event, actor)
    {
      Realm = realm;
      RealmId = realm?.RealmId;

      Key = @event.Key;
      DisplayName = @event.DisplayName;
      Description = @event.Description;

      Subject = @event.Subject;
      ContentType = @event.ContentType;
      Contents = @event.Contents;
    }
    private TemplateEntity() : base()
    {
    }

    public int TemplateId { get; private set; }

    public RealmEntity? Realm { get; private set; }
    public int? RealmId { get; private set; }

    public string Key { get; private set; } = string.Empty;
    public string KeyNormalized
    {
      get => Key.ToUpper();
      private set { }
    }
    public string? DisplayName { get; private set; }
    public string? Description { get; private set; }

    public string Subject { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public string Contents { get; private set; } = string.Empty;

    public RealmEntity? UsedAsPasswordRecoveryTemplateInRealm { get; private set; }

    public void Update(TemplateUpdatedEvent @event, Actor actor)
    {
      base.Update(@event, actor);

      DisplayName = @event.DisplayName;
      Description = @event.Description;

      Subject = @event.Subject;
      ContentType = @event.ContentType;
      Contents = @event.Contents;
    }
  }
}
