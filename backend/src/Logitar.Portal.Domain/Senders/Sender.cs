using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
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
  public TenantId? TenantId => Id.TenantId;
  public EntityId EntityId => Id.EntityId;

  public bool IsDefault { get; private set; }

  private Email? _email = null;
  public Email? Email
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
  private Phone? _phone = null;
  public Phone? Phone
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
  private DisplayName? _displayName = null;
  public DisplayName? DisplayName
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

  public SenderType Type { get; private set; }
  public SenderProvider Provider { get; private set; }
  private SenderSettings? _settings = null;
  public SenderSettings Settings => _settings ?? throw new InvalidOperationException($"The {nameof(Settings)} have not been initialized yet.");

  public Sender() : base()
  {
  }

  public Sender(Email email, SenderSettings settings, ActorId? actorId = null, SenderId? id = null)
    : this(email, phone: null, settings, actorId, id)
  {
  }
  public Sender(Phone phone, SenderSettings settings, ActorId? actorId = null, SenderId? id = null)
    : this(email: null, phone, settings, actorId, id)
  {
  }
  private Sender(Email? email, Phone? phone, SenderSettings settings, ActorId? actorId = null, SenderId? id = null)
    : base((id ?? SenderId.NewId()).StreamId)
  {
    SenderProvider provider = settings.Provider;
    SenderType type = provider.GetSenderType();
    switch (type)
    {
      case SenderType.Email:
        ArgumentNullException.ThrowIfNull(email);
        Raise(new EmailSenderCreated(email, provider), actorId);
        break;
      case SenderType.Sms:
        ArgumentNullException.ThrowIfNull(phone);
        Raise(new SmsSenderCreated(phone, provider), actorId);
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
  protected virtual void Handle(EmailSenderCreated @event)
  {
    Handle((SenderCreated)@event);
  }
  protected virtual void Handle(SenderCreated @event)
  {
    _email = @event.Email;

    Type = @event.Provider.GetSenderType();
    Provider = @event.Provider;
  }
  protected virtual void Handle(SmsSenderCreated @event)
  {
    _phone = @event.Phone;

    Type = @event.Provider.GetSenderType();
    Provider = @event.Provider;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new SenderDeleted(), actorId);
    }
  }

  public void SetDefault(ActorId? actorId = null) => SetDefault(isDefault: true, actorId);
  public void SetDefault(bool isDefault, ActorId? actorId = null)
  {
    if (isDefault != IsDefault)
    {
      Raise(new SenderSetDefault(isDefault), actorId);
    }
  }
  protected virtual void Handle(SenderSetDefault @event)
  {
    IsDefault = @event.IsDefault;
  }

  public void SetSettings(ReadOnlyMailgunSettings settings, ActorId? actorId = null)
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
  protected virtual void Handle(SenderMailgunSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void SetSettings(ReadOnlySendGridSettings settings, ActorId? actorId = null)
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
  protected virtual void Handle(SenderSendGridSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void SetSettings(ReadOnlyTwilioSettings settings, ActorId? actorId = null)
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
  protected virtual void Handle(SenderTwilioSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void Update(ActorId? actorId = null)
  {
    if (_updated.HasChanges)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(SenderUpdated @event)
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
