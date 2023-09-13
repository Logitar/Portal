using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.Templates.Events;
using Logitar.Portal.Domain.Templates.Validators;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Templates;
public class TemplateAggregate : AggregateRoot
{
  private string? _displayName = null;
  private string? _description = null;

  private string _subject = string.Empty;
  private string _contentType = string.Empty;
  private string _contents = string.Empty;

  public TemplateAggregate(AggregateId id) : base(id)
  {
  }

  public TemplateAggregate(IUniqueNameSettings uniqueNameSettings, string uniqueName, string subject, string contentType,
    string contents, string? tenantId = null, ActorId actorId = default, AggregateId? id = null)
      : base(id)
  {
    uniqueName = uniqueName.Trim();
    new UniqueNameValidator(uniqueNameSettings, nameof(UniqueName)).ValidateAndThrow(uniqueName);

    subject = subject.Trim();
    new SubjectValidator(nameof(Subject)).ValidateAndThrow(subject);

    contentType = contentType.Trim();
    new ContentTypeValidator(nameof(ContentType)).ValidateAndThrow(contentType);

    contents = contents.Trim();
    new ContentsValidator(nameof(Contents)).ValidateAndThrow(contents);

    tenantId = tenantId?.CleanTrim();
    if (tenantId != null)
    {
      new TenantIdValidator(nameof(TenantId)).ValidateAndThrow(tenantId);
    }

    ApplyChange(new TemplateCreatedEvent(actorId)
    {
      TenantId = tenantId,
      UniqueName = uniqueName,
      Subject = subject,
      ContentType = contentType,
      Contents = contents
    });
  }
  protected virtual void Apply(TemplateCreatedEvent created)
  {
    TenantId = created.TenantId;

    UniqueName = created.UniqueName;

    _subject = created.Subject;
    _contentType = created.ContentType;
    _contents = created.Contents;
  }

  public string? TenantId { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string? DisplayName
  {
    get => _displayName;
    set
    {
      value = value?.CleanTrim();
      if (value != null)
      {
        new DisplayNameValidator(nameof(DisplayName)).ValidateAndThrow(value);
      }

      if (value != _displayName)
      {
        TemplateUpdatedEvent updated = GetLatestEvent<TemplateUpdatedEvent>();
        updated.DisplayName = new Modification<string>(value);
        _displayName = value;
      }
    }
  }
  public string? Description
  {
    get => _description;
    set
    {
      value = value?.CleanTrim();

      if (value != _description)
      {
        TemplateUpdatedEvent updated = GetLatestEvent<TemplateUpdatedEvent>();
        updated.Description = new Modification<string>(value);
        _description = value;
      }
    }
  }

  public string Subject
  {
    get => _subject;
    set
    {
      value = value.Trim();
      new SubjectValidator(nameof(Subject)).ValidateAndThrow(value);

      if (value != _subject)
      {
        TemplateUpdatedEvent updated = GetLatestEvent<TemplateUpdatedEvent>();
        updated.Subject = value;
        _subject = value;
      }
    }
  }
  public string ContentType
  {
    get => _contentType;
    set
    {
      value = value.Trim();
      new ContentTypeValidator(nameof(ContentType)).ValidateAndThrow(value);

      if (value != _contentType)
      {
        TemplateUpdatedEvent updated = GetLatestEvent<TemplateUpdatedEvent>();
        updated.ContentType = value;
        _contentType = value;
      }
    }
  }
  public string Contents
  {
    get => _contents;
    set
    {
      value = value.Trim();
      new ContentsValidator(nameof(Contents)).ValidateAndThrow(value);

      if (value != _contents)
      {
        TemplateUpdatedEvent updated = GetLatestEvent<TemplateUpdatedEvent>();
        updated.Contents = value;
        _contents = value;
      }
    }
  }

  public void Delete(ActorId actorId = default) => ApplyChange(new TemplateDeletedEvent(actorId));

  public void SetUniqueName(IUniqueNameSettings uniqueNameSettings, string uniqueName)
  {
    uniqueName = uniqueName.Trim();
    new UniqueNameValidator(uniqueNameSettings, nameof(UniqueName)).ValidateAndThrow(uniqueName);

    if (uniqueName != UniqueName)
    {
      TemplateUpdatedEvent updated = GetLatestEvent<TemplateUpdatedEvent>();
      updated.UniqueName = uniqueName;
      UniqueName = uniqueName;
    }
  }

  public void Update(ActorId actorId)
  {
    foreach (DomainEvent change in Changes)
    {
      if (change is TemplateUpdatedEvent updated && updated.ActorId == default)
      {
        updated.ActorId = actorId;

        if (updated.Version == Version)
        {
          UpdatedBy = actorId;
        }
      }
    }
  }

  protected virtual void Apply(TemplateUpdatedEvent updated)
  {
    if (updated.UniqueName != null)
    {
      UniqueName = updated.UniqueName;
    }
    if (updated.DisplayName != null)
    {
      _displayName = updated.DisplayName.Value;
    }
    if (updated.Description != null)
    {
      _description = updated.Description.Value;
    }

    if (updated.Subject != null)
    {
      _subject = updated.Subject;
    }
    if (updated.ContentType != null)
    {
      _contentType = updated.ContentType;
    }
    if (updated.Contents != null)
    {
      _contents = updated.Contents;
    }
  }

  protected virtual T GetLatestEvent<T>() where T : DomainEvent, new()
  {
    T? updated = Changes.SingleOrDefault(change => change is T) as T;
    if (updated == null)
    {
      updated = new();
      ApplyChange(updated);
    }

    return updated;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
