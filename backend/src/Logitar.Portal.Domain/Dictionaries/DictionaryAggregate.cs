using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Dictionaries.Events;
using Logitar.Portal.Domain.Dictionaries.Validators;

namespace Logitar.Portal.Domain.Dictionaries;

public class DictionaryAggregate : AggregateRoot
{
  private DictionaryUpdatedEvent _updatedEvent = new();

  public new DictionaryId Id => new(base.Id);

  public TenantId? TenantId { get; private set; }

  private LocaleUnit? _locale = null;
  public LocaleUnit Locale => _locale ?? throw new InvalidOperationException($"The {nameof(Locale)} have not been initialized yet.");

  private readonly Dictionary<string, string> _entries = [];
  public IReadOnlyDictionary<string, string> Entries => _entries.AsReadOnly();

  public DictionaryAggregate(AggregateId id) : base(id)
  {
  }

  public DictionaryAggregate(LocaleUnit locale, TenantId? tenantId = null, ActorId actorId = default, DictionaryId? id = null)
    : base((id ?? DictionaryId.NewId()).AggregateId)
  {
    Raise(new DictionaryCreatedEvent(actorId, locale, tenantId));
  }
  protected virtual void Apply(DictionaryCreatedEvent @event)
  {
    TenantId = @event.TenantId;

    _locale = @event.Locale;
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new DictionaryDeletedEvent(actorId));
    }
  }

  public void RemoveEntry(string key)
  {
    key = key.Trim();
    if (_entries.ContainsKey(key))
    {
      _updatedEvent.Entries[key] = null;
      _entries.Remove(key);
    }
  }

  private readonly DictionaryEntryValidator _dictionaryEntryValidator = new();
  public void SetEntry(string key, string value)
  {
    key = key.Trim();
    value = value.Trim();
    _dictionaryEntryValidator.ValidateAndThrow(key, value);

    if (!_entries.TryGetValue(key, out string? existingValue) || existingValue != value)
    {
      _updatedEvent.Entries[key] = value;
      _entries[key] = value;
    }
  }

  public void SetLocale(LocaleUnit locale, ActorId actorId = default)
  {
    if (locale != _locale)
    {
      Raise(new DictionaryLocaleChangedEvent(actorId, locale));
    }
  }
  protected virtual void Apply(DictionaryLocaleChangedEvent @event)
  {
    _locale = @event.Locale;
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
  protected virtual void Apply(DictionaryUpdatedEvent @event)
  {
    foreach (KeyValuePair<string, string?> entry in @event.Entries)
    {
      if (entry.Value == null)
      {
        _entries.Remove(entry.Key);
      }
      else
      {
        _entries[entry.Key] = entry.Value;
      }
    }
  }

  public override string ToString() => $"{Locale.Code} | {base.ToString()}";
}
