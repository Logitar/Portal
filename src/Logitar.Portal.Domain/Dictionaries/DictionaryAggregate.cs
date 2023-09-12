using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Domain.Dictionaries.Events;
using Logitar.Portal.Domain.Dictionaries.Validators;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Dictionaries;

public class DictionaryAggregate : AggregateRoot
{
  private readonly Dictionary<string, string> _entries = new();

  public DictionaryAggregate(AggregateId id) : base(id)
  {
  }

  public DictionaryAggregate(Locale locale, string? tenantId = null, ActorId actorId = default, AggregateId? id = null)
    : base(id)
  {
    tenantId = tenantId?.CleanTrim();
    if (tenantId != null)
    {
      new TenantIdValidator(nameof(TenantId)).ValidateAndThrow(tenantId);
    }

    ApplyChange(new DictionaryCreatedEvent(actorId)
    {
      TenantId = tenantId,
      Locale = locale
    });
  }
  protected virtual void Apply(DictionaryCreatedEvent e)
  {
    TenantId = e.TenantId;

    Locale = e.Locale;
  }

  public string? TenantId { get; private set; }

  public Locale Locale { get; private set; } = Locale.Default;

  public IReadOnlyDictionary<string, string> Entries => _entries.AsReadOnly();

  public void Delete(ActorId actorId = default) => ApplyChange(new DictionaryDeletedEvent(actorId));

  public void RemoveEntry(string key)
  {
    key = key.Trim();
    if (_entries.ContainsKey(key))
    {
      DictionaryUpdatedEvent updated = GetLatestEvent<DictionaryUpdatedEvent>();
      updated.Entries[key] = null;
      _entries.Remove(key);
    }
  }

  public void SetEntry(string key, string value)
  {
    key = key.Trim();
    value = value.Trim();
    new DictionaryEntryValidator().ValidateAndThrow(key, value);

    if (!_entries.TryGetValue(key, out string? existingValue) || value != existingValue)
    {
      DictionaryUpdatedEvent updated = GetLatestEvent<DictionaryUpdatedEvent>();
      updated.Entries[key] = value;
      _entries[key] = value;
    }
  }

  public void Update(ActorId actorId)
  {
    foreach (DomainEvent change in Changes)
    {
      if (change is DictionaryUpdatedEvent updated && updated.ActorId == default)
      {
        updated.ActorId = actorId;

        if (updated.Version == Version)
        {
          UpdatedBy = actorId;
        }
      }
    }
  }

  protected virtual void Apply(DictionaryUpdatedEvent updated)
  {
    foreach (KeyValuePair<string, string?> entry in updated.Entries)
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
}
