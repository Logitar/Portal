using Logitar.EventSourcing;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders.Events;
using Logitar.Portal.Domain.Senders.Mailgun;
using Logitar.Portal.Domain.Senders.SendGrid;
using Logitar.Portal.Domain.Senders.Twilio;

namespace Logitar.Portal.Domain.Senders;

public class SenderAggregate : AggregateRoot
{
  private SenderUpdatedEvent _updatedEvent = new();

  public new SenderId Id => new(base.Id);

  public TenantId? TenantId { get; private set; }

  public bool IsDefault { get; private set; }

  private EmailUnit? _email = null;
  public EmailUnit? Email
  {
    get => _email;
    set
    {
      if (Type != SenderType.Email)
      {
        throw new InvalidOperationException($"The sender must be an {nameof(SenderType.Email)} sender in order to set its email address.");
      }
      else if (value != _email)
      {
        _email = value;
        _updatedEvent.Email = value;
      }
    }
  }
  private PhoneUnit? _phone = null;
  public PhoneUnit? Phone
  {
    get => _phone;
    set
    {
      if (Type != SenderType.Sms)
      {
        throw new InvalidOperationException($"The sender must be an {nameof(SenderType.Sms)} sender in order to set its phone number.");
      }
      else if (value != _phone)
      {
        _phone = value;
        _updatedEvent.Phone = value;
      }
    }
  }
  private DisplayNameUnit? _displayName = null;
  public DisplayNameUnit? DisplayName
  {
    get => _displayName;
    set
    {
      if (Type != SenderType.Email)
      {
        throw new InvalidOperationException($"The sender must be an {nameof(SenderType.Email)} sender in order to set its display name.");
      }
      else if (value != _displayName)
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

  public SenderType Type { get; private set; }
  public SenderProvider Provider { get; private set; }
  private SenderSettings? _settings = null;
  public SenderSettings Settings => _settings ?? throw new InvalidOperationException($"The {nameof(Settings)} have not been initialized yet.");

  public SenderAggregate(AggregateId id) : base(id)
  {
  }

  public SenderAggregate(EmailUnit email, SenderSettings settings, TenantId? tenantId = null, ActorId actorId = default, SenderId? id = null)
    : this(email, phone: null, settings, tenantId, actorId, id)
  {
  }
  public SenderAggregate(PhoneUnit phone, SenderSettings settings, TenantId? tenantId = null, ActorId actorId = default, SenderId? id = null)
    : this(email: null, phone, settings, tenantId, actorId, id)
  {
  }
  private SenderAggregate(EmailUnit? email, PhoneUnit? phone, SenderSettings settings, TenantId? tenantId = null, ActorId actorId = default, SenderId? id = null)
    : base((id ?? SenderId.NewId()).AggregateId)
  {
    SenderProvider provider = settings.Provider;
    SenderType type = provider.GetSenderType();
    switch (type)
    {
      case SenderType.Email:
        ArgumentNullException.ThrowIfNull(email);
        Raise(new EmailSenderCreatedEvent(actorId, email, provider, tenantId));
        break;
      case SenderType.Sms:
        ArgumentNullException.ThrowIfNull(phone);
        Raise(new SmsSenderCreatedEvent(actorId, phone, provider, tenantId));
        break;
      default:
        throw new NotSupportedException($"The sender type '{type}' is not supported.");
    }

    switch (provider)
    {
      case SenderProvider.Mailgun:
        SetSettings((ReadOnlyMailgunSettings)settings);
        break;
      case SenderProvider.SendGrid:
        SetSettings((ReadOnlySendGridSettings)settings);
        break;
      case SenderProvider.Twilio:
        SetSettings((ReadOnlyTwilioSettings)settings);
        break;
      default:
        throw new SenderProviderNotSupportedException(provider);
    }
  }
  protected virtual void Apply(EmailSenderCreatedEvent @event)
  {
    Apply((SenderCreatedEvent)@event);
  }
  protected virtual void Apply(SenderCreatedEvent @event)
  {
    TenantId = @event.TenantId;

    _email = @event.Email;

    Type = @event.Provider.GetSenderType();
    Provider = @event.Provider;
  }
  protected virtual void Apply(SmsSenderCreatedEvent @event)
  {
    TenantId = @event.TenantId;

    _phone = @event.Phone;

    Type = @event.Provider.GetSenderType();
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

  public void SetSettings(ReadOnlyMailgunSettings settings, ActorId actorId = default)
  {
    if (Provider != SenderProvider.Mailgun)
    {
      throw new SenderProviderMismatchException(this, settings.Provider);
    }
    else if (settings != _settings)
    {
      Raise(new SenderMailgunSettingsChangedEvent(actorId, settings));
    }
  }
  protected virtual void Apply(SenderMailgunSettingsChangedEvent @event)
  {
    _settings = @event.Settings;
  }

  public void SetSettings(ReadOnlySendGridSettings settings, ActorId actorId = default)
  {
    if (Provider != SenderProvider.SendGrid)
    {
      throw new SenderProviderMismatchException(this, settings.Provider);
    }
    else if (settings != _settings)
    {
      Raise(new SenderSendGridSettingsChangedEvent(actorId, settings));
    }
  }
  protected virtual void Apply(SenderSendGridSettingsChangedEvent @event)
  {
    _settings = @event.Settings;
  }

  public void SetSettings(ReadOnlyTwilioSettings settings, ActorId actorId = default)
  {
    if (Provider != SenderProvider.Twilio)
    {
      throw new SenderProviderMismatchException(this, settings.Provider);
    }
    else if (settings != _settings)
    {
      Raise(new SenderTwilioSettingsChangedEvent(actorId, settings));
    }
  }
  protected virtual void Apply(SenderTwilioSettingsChangedEvent @event)
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
    if (@event.Phone != null)
    {
      _phone = @event.Phone;
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
    switch (Type)
    {
      case SenderType.Email:
        if (Email == null)
        {
          throw new InvalidOperationException($"The {nameof(Email)} has not been initialized yet.");
        }
        if (DisplayName == null)
        {
          s.Append(Email.Address);
        }
        else
        {
          s.Append(DisplayName.Value).Append(" <").Append(Email.Address).Append('>');
        }
        break;
      case SenderType.Sms:
        if (Phone == null)
        {
          throw new InvalidOperationException($"The {nameof(Phone)} has not been initialized yet.");
        }
        s.Append(Phone.FormatToE164());
        break;
      default:
        throw new NotSupportedException($"The sender type '{Type}' is not supported.");
    }
    s.Append(" | ");
    s.Append(base.ToString());
    return s.ToString();
  }
}
