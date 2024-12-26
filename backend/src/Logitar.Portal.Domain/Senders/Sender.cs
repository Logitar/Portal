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

public class Sender : AggregateRoot
{
  private SenderUpdated _updated = new();

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
      else if (_email != value)
      {
        _email = value;
        _updated.Email = value;
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
      else if (_phone != value)
      {
        _phone = value;
        _updated.Phone = value;
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
      else if (_displayName != value)
      {
        _displayName = value;
        _updated.DisplayName = new Modification<DisplayNameUnit>(value);
      }
    }
  }
  private DescriptionUnit? _description = null;
  public DescriptionUnit? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updated.Description = new Modification<DescriptionUnit>(value);
      }
    }
  }

  public SenderType Type { get; private set; }
  public SenderProvider Provider { get; private set; }
  private SenderSettings? _settings = null;
  public SenderSettings Settings => _settings ?? throw new InvalidOperationException($"The {nameof(Settings)} have not been initialized yet.");

  public Sender(AggregateId id) : base(id)
  {
  }

  public Sender(EmailUnit email, SenderSettings settings, TenantId? tenantId = null, ActorId actorId = default, SenderId? id = null)
    : this(email, phone: null, settings, tenantId, actorId, id)
  {
  }
  public Sender(PhoneUnit phone, SenderSettings settings, TenantId? tenantId = null, ActorId actorId = default, SenderId? id = null)
    : this(email: null, phone, settings, tenantId, actorId, id)
  {
  }
  private Sender(EmailUnit? email, PhoneUnit? phone, SenderSettings settings, TenantId? tenantId = null, ActorId actorId = default, SenderId? id = null)
    : base((id ?? SenderId.NewId()).AggregateId)
  {
    SenderProvider provider = settings.Provider;
    SenderType type = provider.GetSenderType();
    switch (type)
    {
      case SenderType.Email:
        ArgumentNullException.ThrowIfNull(email);
        Raise(new EmailSenderCreated(tenantId, email, provider), actorId);
        break;
      case SenderType.Sms:
        ArgumentNullException.ThrowIfNull(phone);
        Raise(new SmsSenderCreated(tenantId, phone, provider), actorId);
        break;
      default:
        throw new SenderTypeNotSupportedException(type);
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
  protected virtual void Apply(EmailSenderCreated @event)
  {
    Apply((SenderCreated)@event);
  }
  protected virtual void Apply(SenderCreated @event)
  {
    TenantId = @event.TenantId;

    _email = @event.Email;

    Type = @event.Provider.GetSenderType();
    Provider = @event.Provider;
  }
  protected virtual void Apply(SmsSenderCreated @event)
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
      Raise(new SenderDeleted(), actorId);
    }
  }

  public void SetDefault(ActorId actorId = default) => SetDefault(isDefault: true, actorId);
  public void SetDefault(bool isDefault, ActorId actorId = default)
  {
    if (isDefault != IsDefault)
    {
      Raise(new SenderSetDefault(isDefault), actorId);
    }
  }
  protected virtual void Apply(SenderSetDefault @event)
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
      Raise(new SenderMailgunSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Apply(SenderMailgunSettingsChanged @event)
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
      Raise(new SenderSendGridSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Apply(SenderSendGridSettingsChanged @event)
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
      Raise(new SenderTwilioSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Apply(SenderTwilioSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void Update(ActorId actorId = default)
  {
    if (_updated.HasChanges)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Apply(SenderUpdated @event)
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
        throw new SenderTypeNotSupportedException(Type);
    }
    s.Append(" | ");
    s.Append(base.ToString());
    return s.ToString();
  }
}
