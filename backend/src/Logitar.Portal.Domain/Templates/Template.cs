using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Templates.Events;

namespace Logitar.Portal.Domain.Templates
{
  public class Template : AggregateRoot
  {
    public Template(AggregateId actorId, string key, string subject, string contentType, string contents,
      Realm? realm = null, string? displayName = null, string? description = null) : base()
    {
      ApplyChange(new TemplateCreatedEvent
      {
        RealmId = realm?.Id,
        Key = key.Trim(),
        DisplayName = displayName?.CleanTrim(),
        Description = description?.CleanTrim(),
        Subject = subject.Trim(),
        ContentType = contentType.Trim(),
        Contents = contents.Trim()
      }, actorId);
    }
    private Template() : base()
    {
    }

    public AggregateId? RealmId { get; private set; }

    public string Key { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }
    public string? Description { get; private set; }

    public string Subject { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public string Contents { get; private set; } = string.Empty;

    public void Delete(AggregateId actorId) => ApplyChange(new TemplateDeletedEvent(), actorId);
    public void Update(AggregateId actorId, string subject, string contentType, string contents, string? displayName = null, string? description = null)
    {
      ApplyChange(new TemplateUpdatedEvent
      {
        DisplayName = displayName?.CleanTrim(),
        Description = description?.CleanTrim(),
        Subject = subject.Trim(),
        ContentType = contentType.Trim(),
        Contents = contents.Trim()
      }, actorId);
    }

    protected virtual void Apply(TemplateCreatedEvent @event)
    {
      RealmId = @event.RealmId;

      Key = @event.Key;
      DisplayName = @event.DisplayName;
      Description = @event.Description;

      Subject = @event.Subject;
      ContentType = @event.ContentType;
      Contents = @event.Contents;
    }
    protected virtual void Apply(TemplateDeletedEvent @event)
    {
      Delete();
    }
    protected virtual void Apply(TemplateUpdatedEvent @event)
    {
      DisplayName = @event.DisplayName;
      Description = @event.Description;

      Subject = @event.Subject;
      ContentType = @event.ContentType;
      Contents = @event.Contents;
    }

    public override string ToString() => $"{DisplayName ?? Key} | {base.ToString()}";
  }
}
