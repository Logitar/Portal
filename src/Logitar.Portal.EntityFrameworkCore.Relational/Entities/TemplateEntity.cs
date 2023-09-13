using Logitar.Portal.Domain.Templates.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record TemplateEntity : AggregateEntity
{
  public TemplateEntity(TemplateCreatedEvent created) : base(created)
  {
    TenantId = created.TenantId;

    UniqueName = created.UniqueName;

    Subject = created.Subject;
    ContentType = created.ContentType;
    Contents = created.Contents;
  }

  private TemplateEntity() : base()
  {
  }

  public int TemplateId { get; private set; }

  public string? TenantId { get; private set; }

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

  public RealmEntity? PasswordRecoveryInRealm { get; private set; }

  public void Update(TemplateUpdatedEvent updated)
  {
    base.Update(updated);

    if (updated.UniqueName != null)
    {
      UniqueName = updated.UniqueName;
    }
    if (updated.DisplayName != null)
    {
      DisplayName = updated.DisplayName.Value;
    }
    if (updated.Description != null)
    {
      Description = updated.Description.Value;
    }

    if (updated.Subject != null)
    {
      Subject = updated.Subject;
    }
    if (updated.ContentType != null)
    {
      ContentType = updated.ContentType;
    }
    if (updated.Contents != null)
    {
      Contents = updated.Contents;
    }
  }
}
