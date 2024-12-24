using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Templates.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal class TemplateEntity : AggregateEntity
{
  public int TemplateId { get; private set; }

  public string? TenantId { get; private set; } = string.Empty;
  public string EntityId { get; private set; } = string.Empty;

  public string UniqueKey { get; private set; } = string.Empty;
  public string UniqueKeyNormalized
  {
    get => Helper.Normalize(UniqueKey);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string Subject { get; private set; } = string.Empty;
  public string ContentType { get; private set; } = string.Empty;
  public string ContentText { get; private set; } = string.Empty;

  public List<MessageEntity> Messages { get; private set; } = [];

  public TemplateEntity(TemplateCreated @event) : base(@event)
  {
    TemplateId templateId = new(@event.StreamId);
    TenantId = templateId.TenantId?.Value;
    EntityId = templateId.EntityId.Value;

    UniqueKey = @event.UniqueKey.Value;

    Subject = @event.Subject.Value;
    SetContent(@event.Content);
  }

  private TemplateEntity() : base()
  {
  }

  public void SetUniqueKey(TemplateUniqueKeyChanged @event)
  {
    Update(@event);

    UniqueKey = @event.UniqueKey.Value;
  }

  public void Update(TemplateUpdated @event)
  {
    base.Update(@event);

    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.Subject != null)
    {
      Subject = @event.Subject.Value;
    }
    if (@event.Content != null)
    {
      SetContent(@event.Content);
    }
  }

  private void SetContent(Content content)
  {
    ContentType = content.Type;
    ContentText = content.Text;
  }

  public override string ToString() => $"{DisplayName ?? UniqueKey} | {base.ToString()}";
}
