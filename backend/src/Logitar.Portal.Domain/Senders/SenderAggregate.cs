using Logitar.EventSourcing;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders.Events;
using Logitar.Portal.Domain.Senders.SendGrid;

namespace Logitar.Portal.Domain.Senders;

public class SenderAggregate : AggregateRoot
{
  private SenderUpdatedEvent _updatedEvent = new();

  public new SenderId Id => new(base.Id);

  public TenantId? TenantId { get; private set; }

  public bool IsDefault { get; private set; }

  private EmailUnit? _email = null;
  public EmailUnit Email
  {
    get => _email ?? throw new InvalidOperationException($"The {nameof(Email)} has not been initialized yet.");
    set
    {
      if (value != _email)
      {
        _email = value;
        _updatedEvent.Email = value;
      }
    }
  }
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

  public SenderProvider Provider { get; private set; }
  private SenderSettings? _settings = null;
  public SenderSettings Settings => _settings ?? throw new InvalidOperationException($"The {nameof(Settings)} have not been initialized yet.");

  public SenderAggregate(AggregateId id) : base(id)
  {
  }

  public SenderAggregate(EmailUnit email, SenderSettings settings, TenantId? tenantId = null, ActorId actorId = default, SenderId? id = null)
    : base((id ?? SenderId.NewId()).AggregateId)
  {
    Raise(new SenderCreatedEvent(actorId, email, settings.Provider, tenantId));

    switch (settings.Provider)
    {
      case SenderProvider.SendGrid:
        SetSettings((ReadOnlySendGridSettings)settings);
        break;
      default:
        throw new SenderProviderNotSupportedException(settings.Provider);
    }
  }
  protected virtual void Apply(SenderCreatedEvent @event)
  {
    TenantId = @event.TenantId;

    _email = @event.Email;

    Provider = @event.Provider;
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new SenderDeletedEvent(actorId));
    }
  }

  public void SetDefault(ActorId actorId = default) => SetDefault(isDefault: true, actorId);
  public void SetDefault(bool isDefault, ActorId actorId = default)
  {
    if (isDefault != IsDefault)
    {
      Raise(new SenderSetDefaultEvent(actorId, isDefault));
    }
  }
  protected virtual void Apply(SenderSetDefaultEvent @event)
  {
    IsDefault = @event.IsDefault;
  }

  public void SetSettings(ReadOnlySendGridSettings settings, ActorId actorId = default)
  {
    if (Provider != SenderProvider.SendGrid)
    {
      throw new SenderProviderMismatchException(this, settings.Provider);
    }

    Raise(new SenderSendGridSettingsChangedEvent(actorId, settings));
  }
  protected virtual void Apply(SenderSendGridSettingsChangedEvent @event)
  {
    _settings = @event.Settings;
  }

  public void Update(ActorId actorId = default)
  {
    if (_updatedEvent.HasChanges)
    {
      _updatedEvent.ActorId = actorId;
      Raise(_updatedEvent);
      _updatedEvent = new();
    }
  }
  protected virtual void Apply(SenderUpdatedEvent @event)
  {
    if (@event.Email != null)
    {
      _email = @event.Email;
    }
    if (@event.DisplayName != null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description != null)
    {
      _description = @event.Description.Value;
    }
  }

  public override string ToString()
  {
    StringBuilder s = new();
    if (DisplayName == null)
    {
      s.Append(Email.Address);
    }
    else
    {
      s.Append(DisplayName.Value).Append(" <").Append(Email.Address).Append('>');
    }
    s.Append(" | ");
    s.Append(base.ToString());
    return s.ToString();
  }
}
