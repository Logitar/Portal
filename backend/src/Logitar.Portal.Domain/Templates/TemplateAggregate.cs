using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Domain.Templates.Events;

namespace Logitar.Portal.Domain.Templates;

public class TemplateAggregate : AggregateRoot
{
  private TemplateUpdated _updated = new();

  public new TemplateId Id => new(base.Id);
  public TenantId? TenantId => Id.TenantId;
  public EntityId EntityId => Id.EntityId;

  private Identifier? _uniqueKey = null;
  public Identifier UniqueKey => _uniqueKey ?? throw new InvalidOperationException($"The {nameof(UniqueKey)} has not been initialized yet.");
  private DisplayName? _displayName = null;
  public DisplayName? DisplayName
  {
    get => _displayName;
    set
    {
      if (_displayName != value)
      {
        _displayName = value;
        _updated.DisplayName = new Change<DisplayName>(value);
      }
    }
  }
  private Description? _description = null;
  public Description? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updated.Description = new Change<Description>(value);
      }
    }
  }

  private Subject? _subject = null;
  public Subject Subject
  {
    get => _subject ?? throw new InvalidOperationException($"The {nameof(Subject)} has not been initialized yet.");
    set
    {
      if (_subject != value)
      {
        _subject = value;
        _updated.Subject = value;
      }
    }
  }
  private Content? _content = null;
  public Content Content
  {
    get => _content ?? throw new InvalidOperationException($"The {nameof(Content)} has not been initialized yet.");
    set
    {
      if (_content != value)
      {
        _content = value;
        _updated.Content = value;
      }
    }
  }

  public TemplateAggregate() : base()
  {
  }

  public TemplateAggregate(Identifier uniqueKey, Subject subject, Content content, ActorId actorId = default, TemplateId? id = null) : base((id ?? TemplateId.NewId()).StreamId)
  {
    Raise(new TemplateCreated(uniqueKey, subject, content), actorId);
  }
  protected virtual void Handle(TemplateCreated @event)
  {
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

  public void SetUniqueKey(Identifier uniqueKey, ActorId actorId = default)
  {
    if (uniqueKey != _uniqueKey)
    {
      Raise(new TemplateUniqueKeyChanged(uniqueKey), actorId);
    }
  }
  protected virtual void Handle(TemplateUniqueKeyChanged @event)
  {
    _uniqueKey = @event.UniqueKey;
  }

  public void Update(ActorId actorId = default)
  {
    if (_updated.HasChanges)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(TemplateUpdated @event)
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
