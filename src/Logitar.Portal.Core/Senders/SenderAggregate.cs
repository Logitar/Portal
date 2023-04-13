using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Senders.Events;
using Logitar.Portal.Core.Senders.Validators;
using System.Text;

namespace Logitar.Portal.Core.Senders;

public class SenderAggregate : AggregateRoot
{
  private readonly Dictionary<string, string> _settings = new();

  public SenderAggregate(AggregateId id) : base(id)
  {
  }

  public SenderAggregate(AggregateId actorId, RealmAggregate realm, ProviderType provider,
    string emailAddress, string? displayName = null, Dictionary<string, string>? settings = null) : base()
  {
    SenderCreated e = new()
    {
      ActorId = actorId,
      RealmId = realm.Id,
      Provider = provider,
      EmailAddress = emailAddress.Trim(),
      DisplayName = displayName?.CleanTrim(),
      Settings = settings?.CleanTrim() ?? new()
    };
    new SenderCreatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }

  public AggregateId RealmId { get; private set; }

  public ProviderType Provider { get; private set; }

  public bool IsDefault { get; private set; }

  public string EmailAddress { get; private set; } = string.Empty;
  public string? DisplayName { get; private set; }

  public IReadOnlyDictionary<string, string> Settings => _settings.AsReadOnly();

  protected virtual void Apply(SenderCreated e)
  {
    RealmId = e.RealmId;

    Provider = e.Provider;

    Apply((SenderSaved)e);
  }

  public void Delete(AggregateId actorId) => ApplyChange(new SenderDeleted { ActorId = actorId });
  protected virtual void Apply(SenderDeleted _) { }

  public void SetDefault(AggregateId actorId, bool isDefault = true)
  {
    if (IsDefault != isDefault)
    {
      ApplyChange(new SenderSetDefault
      {
        ActorId = actorId,
        IsDefault = isDefault
      });
    }
  }
  protected virtual void Apply(SenderSetDefault e) => IsDefault = e.IsDefault;

  public void Update(AggregateId actorId, string emailAddress, string? displayName = null,
    Dictionary<string, string>? settings = null)
  {
    SenderUpdated e = new()
    {
      ActorId = actorId,
      EmailAddress = emailAddress.Trim(),
      DisplayName = displayName?.CleanTrim(),
      Settings = settings?.CleanTrim() ?? new()
    };
    new SenderUpdatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  protected virtual void Apply(SenderUpdated e) => Apply((SenderSaved)e);

  private void Apply(SenderSaved e)
  {
    EmailAddress = e.EmailAddress;
    DisplayName = e.DisplayName;

    _settings.Clear();
    _settings.AddRange(e.Settings);
  }

  public override string ToString()
  {
    StringBuilder s = new();

    if (DisplayName == null)
    {
      s.Append(EmailAddress);
    }
    else
    {
      s.Append(DisplayName);
      s.Append(" <");
      s.Append(EmailAddress);
      s.Append('>');
    }

    s.Append(" | ");
    s.Append(base.ToString());

    return s.ToString();
  }
}
