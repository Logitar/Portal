using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Core.Dictionaries.Events;
using Logitar.Portal.Core.Dictionaries.Validators;
using Logitar.Portal.Core.Realms;
using System.Globalization;

namespace Logitar.Portal.Core.Dictionaries;

public class DictionaryAggregate : AggregateRoot
{
  private readonly Dictionary<string, string> _entries = new();

  public DictionaryAggregate(AggregateId id) : base(id)
  {
  }

  public DictionaryAggregate(AggregateId actorId, RealmAggregate realm, CultureInfo locale,
    Dictionary<string, string>? entries = null) : base()
  {
    DictionaryCreated e = new()
    {
      ActorId = actorId,
      RealmId = realm.Id,
      Locale = locale,
      Entries = entries?.CleanTrim() ?? new()
    };
    new DictionaryCreatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }

  public AggregateId RealmId { get; private set; }
  public CultureInfo Locale { get; private set; } = null!;

  public IReadOnlyDictionary<string, string> Entries => _entries.AsReadOnly();

  protected virtual void Apply(DictionaryCreated e)
  {
    RealmId = e.RealmId;
    Locale = e.Locale;

    Apply((DictionarySaved)e);
  }

  public void Delete(AggregateId actorId) => ApplyChange(new DictionaryDeleted { ActorId = actorId });
  protected virtual void Apply(DictionaryDeleted _) { }

  public void Update(AggregateId actorId, Dictionary<string, string>? entries)
  {
    DictionaryUpdated e = new()
    {
      ActorId = actorId,
      Entries = entries?.CleanTrim() ?? new()
    };
    new DictionaryUpdatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  protected virtual void Apply(DictionaryUpdated e) => Apply((DictionarySaved)e);

  private void Apply(DictionarySaved e)
  {
    _entries.Clear();
    _entries.AddRange(e.Entries);
  }
}
