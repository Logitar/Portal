using Logitar.Portal.Domain.Dictionaries.Events;
using Logitar.Portal.Domain.Realms;
using System.Globalization;

namespace Logitar.Portal.Domain.Dictionaries
{
  public class Dictionary : AggregateRoot
  {
    public Dictionary(AggregateId actorId, CultureInfo locale, Realm? realm = null, Dictionary<string, string>? entries = null) : base()
    {
      ApplyChange(new DictionaryCreatedEvent
      {
        RealmId = realm?.Id,
        Locale = locale,
        Entries = entries
      }, actorId);
    }
    private Dictionary() : base()
    {
    }

    public AggregateId? RealmId { get; private set; }

    public CultureInfo Locale { get; private set; } = CultureInfo.InvariantCulture;

    public Dictionary<string, string> Entries { get; private set; } = new();

    public void Delete(AggregateId actorId) => ApplyChange(new DictionaryDeletedEvent(), actorId);
    public void Update(AggregateId actorId, Dictionary<string, string>? entries = null) => ApplyChange(new DictionaryUpdatedEvent
    {
      Entries = entries
    }, actorId);

    protected virtual void Apply(DictionaryCreatedEvent @event)
    {
      RealmId = @event.RealmId;

      Locale = @event.Locale;

      Entries = @event.Entries ?? new();
    }
    protected virtual void Apply(DictionaryDeletedEvent @event)
    {
      Delete();
    }
    protected virtual void Apply(DictionaryUpdatedEvent @event)
    {
      Entries = @event.Entries ?? new();
    }
  }
}
