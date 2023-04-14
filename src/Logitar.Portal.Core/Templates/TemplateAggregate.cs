using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Templates.Events;
using Logitar.Portal.Core.Templates.Validators;

namespace Logitar.Portal.Core.Templates;

public class TemplateAggregate : AggregateRoot
{
  public TemplateAggregate(AggregateId id) : base(id)
  {
  }

  public TemplateAggregate(AggregateId actorId, RealmAggregate realm, string uniqueName, string subject,
    string contentType, string contents, string? displayName = null, string? description = null) : base()
  {
    TemplateCreated e = new()
    {
      ActorId = actorId,
      RealmId = realm.Id,
      UniqueName = uniqueName.Trim(),
      DisplayName = displayName?.CleanTrim(),
      Description = description?.CleanTrim(),
      Subject = subject.Trim(),
      ContentType = contentType.Trim(),
      Contents = contents.Trim()
    };
    new TemplateCreatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }

  public AggregateId RealmId { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string Subject { get; private set; } = string.Empty;
  public string ContentType { get; private set; } = string.Empty;
  public string Contents { get; private set; } = string.Empty;

  protected virtual void Apply(TemplateCreated e)
  {
    RealmId = e.RealmId;

    UniqueName = e.UniqueName;

    Apply((TemplateSaved)e);
  }

  public void Delete(AggregateId actorId) => ApplyChange(new TemplateDeleted { ActorId = actorId });
  protected virtual void Apply(TemplateDeleted _) { }

  public void Update(AggregateId actorId, string subject, string contentType,
    string contents, string? displayName = null, string? description = null)
  {
    TemplateUpdated e = new()
    {
      ActorId = actorId,
      DisplayName = displayName?.CleanTrim(),
      Description = description?.CleanTrim(),
      Subject = subject.Trim(),
      ContentType = contentType.Trim(),
      Contents = contents.Trim()
    };
    new TemplateUpdatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  protected virtual void Apply(TemplateUpdated e) => Apply((TemplateSaved)e);

  private void Apply(TemplateSaved e)
  {
    DisplayName = e.DisplayName;
    Description = e.Description;

    Subject = e.Subject;
    ContentType = e.ContentType;
    Contents = e.Contents;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
