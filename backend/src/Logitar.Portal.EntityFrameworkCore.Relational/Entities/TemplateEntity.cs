﻿using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal class TemplateEntity : AggregateEntity
{
  public int TemplateId { get; private set; }

  public string? TenantId { get; private set; }

  public string UniqueKey { get; private set; } = string.Empty;
  public string UniqueKeyNormalized
  {
    get => UniqueKey.ToUpper();
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string Subject { get; private set; } = string.Empty;
  public string ContentType { get; private set; } = string.Empty;
  public string ContentText { get; private set; } = string.Empty;

  public List<MessageEntity> Messages { get; private set; } = [];

  public TemplateEntity(TemplateCreatedEvent @event) : base(@event)
  {
    TenantId = @event.TenantId?.Value;

    UniqueKey = @event.UniqueKey.Value;

    Subject = @event.Subject.Value;
    SetContent(@event.Content);
  }

  private TemplateEntity() : base()
  {
  }

  public void SetUniqueKey(TemplateUniqueKeyChangedEvent @event)
  {
    Update(@event);

    UniqueKey = @event.UniqueKey.Value;
  }

  public void Update(TemplateUpdatedEvent @event)
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

  private void SetContent(IContent content)
  {
    ContentType = content.Type;
    ContentText = content.Text;
  }
}
