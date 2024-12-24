using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Domain.Dictionaries.Events;

namespace Logitar.Portal.Domain.Dictionaries;

public class Dictionary : AggregateRoot
{
  private DictionaryUpdated _updated = new();

  public new DictionaryId Id => new(base.Id);
  public TenantId? TenantId => Id.TenantId;
  public EntityId EntityId => Id.EntityId;

  private Locale? _locale = null;
  public Locale Locale => _locale ?? throw new InvalidOperationException($"The {nameof(Locale)} has not been initialized yet.");

  private readonly Dictionary<Identifier, string> _entries = [];
  public IReadOnlyDictionary<Identifier, string> Entries => _entries.AsReadOnly();

  public Dictionary() : base()
  {
  }

  public Dictionary(Locale locale, ActorId? actorId = null, DictionaryId? id = null) : base((id ?? DictionaryId.NewId()).StreamId)
  {
    Raise(new DictionaryCreated(locale), actorId);
  }
  protected virtual void Handle(DictionaryCreated @event)
  {
    _locale = @event.Locale;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new DictionaryDeleted(), actorId);
    }
  }

  public void RemoveEntry(Identifier key)
  {
    if (_entries.Remove(key))
    {
      _updated.Entries[key] = null;
    }
  }

  public void SetEntry(Identifier key, string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      RemoveEntry(key);
    }
    value = value.Trim();

    if (!_entries.TryGetValue(key, out string? existingValue) || existingValue != value)
    {
      _updated.Entries[key] = value;
      _entries[key] = value;
    }
  }

  public void SetLocale(Locale locale, ActorId? actorId = null)
  {
    if (_locale != locale)
    {
      Raise(new DictionaryLocaleChanged(locale), actorId);
    }
  }
  protected virtual void Handle(DictionaryLocaleChanged @event)
  {
    _locale = @event.Locale;
  }

  public string? Translate(Identifier key) => _entries.TryGetValue(key, out string? value) ? value : null;

  public void Update(ActorId? actorId = null)
  {
    if (_updated.HasChanges)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(DictionaryUpdated @event)
  {
    foreach (KeyValuePair<Identifier, string?> entry in @event.Entries)
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

  public override string ToString() => $"{Locale} | {base.ToString()}";
}
