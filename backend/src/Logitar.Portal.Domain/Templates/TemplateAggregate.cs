using Logitar.EventSourcing;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Templates.Events;

namespace Logitar.Portal.Domain.Templates;

public class TemplateAggregate : AggregateRoot
{
  private TemplateUpdated _updatedEvent = new();

  public new TemplateId Id => new(base.Id);

  public TenantId? TenantId { get; private set; }

  private UniqueKeyUnit? _uniqueKey = null;
  public UniqueKeyUnit UniqueKey => _uniqueKey ?? throw new InvalidOperationException($"The {nameof(UniqueKey)} has not been initialized yet.");
  private DisplayNameUnit? _displayName = null;
  public DisplayNameUnit? DisplayName
  {
    get => _displayName;
    set
    {
      if (value != _displayName)
      {
        _displayName = value;
        _updatedEvent.DisplayName = new Modification<DisplayNameUnit>(value);
      }
    }
  }
  private DescriptionUnit? _description = null;
  public DescriptionUnit? Description
  {
    get => _description;
    set
    {
      if (value != _description)
      {
        _description = value;
        _updatedEvent.Description = new Modification<DescriptionUnit>(value);
      }
    }
  }

  private SubjectUnit? _subject = null;
  public SubjectUnit Subject
  {
    get => _subject ?? throw new InvalidOperationException($"The {nameof(Subject)} has not been initialized yet.");
    set
    {
      if (value != _subject)
      {
        _subject = value;
        _updatedEvent.Subject = value;
      }
    }
  }
  private ContentUnit? _content = null;
  public ContentUnit Content
  {
    get => _content ?? throw new InvalidOperationException($"The {nameof(Content)} has not been initialized yet.");
    set
    {
      if (value != _content)
      {
        _content = value;
        _updatedEvent.Content = value;
      }
    }
  }

  public TemplateAggregate(AggregateId id) : base(id)
  {
  }

  public TemplateAggregate(UniqueKeyUnit uniqueKey, SubjectUnit subject, ContentUnit content, TenantId? tenantId = null,
    ActorId actorId = default, TemplateId? id = null) : base((id ?? TemplateId.NewId()).AggregateId)
  {
    Raise(new TemplateCreated(tenantId, uniqueKey, subject, content), actorId);
  }
  protected virtual void Apply(TemplateCreated @event)
  {
    TenantId = @event.TenantId;

    _uniqueKey = @event.UniqueKey;

    _subject = @event.Subject;
    _content = @event.Content;
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new TemplateDeleted(), actorId);
    }
  }

  public void SetUniqueKey(UniqueKeyUnit uniqueKey, ActorId actorId = default)
  {
    if (uniqueKey != _uniqueKey)
    {
      Raise(new TemplateUniqueKeyChanged(uniqueKey), actorId);
    }
  }
  protected virtual void Apply(TemplateUniqueKeyChanged @event)
  {
    _uniqueKey = @event.UniqueKey;
  }

  public void Update(ActorId actorId = default)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, actorId, DateTime.Now);
      _updatedEvent = new();
    }
  }
  protected virtual void Apply(TemplateUpdated @event)
  {
    if (@event.DisplayName != null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description != null)
    {
      _description = @event.Description.Value;
    }

    if (@event.Subject != null)
    {
      _subject = @event.Subject;
    }
    if (@event.Content != null)
    {
      _content = @event.Content;
    }
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueKey.Value}";
}
