using Logitar.Portal.Core.Dictionaries.Events;
using Logitar.Portal.Core.Dictionaries.Payloads;
using Logitar.Portal.Core.Realms;
using System.Globalization;
using System.Text.Json;

namespace Logitar.Portal.Core.Dictionaries
{
  public class Dictionary : Aggregate
  {
    public Dictionary(CreateDictionaryPayload payload, Guid userId, Realm? realm = null)
    {
      ApplyChange(new CreatedEvent(payload, userId));

      Realm = realm;
      RealmSid = realm?.Sid;
    }
    private Dictionary()
    {
    }

    public Realm? Realm { get; private set; }
    public int? RealmSid { get; private set; }

    public CultureInfo Culture => CultureInfo.GetCultureInfo(Locale);
    public string Locale { get; private set; } = null!;

    public Dictionary<string, string> Entries { get; private set; } = new();
    public string? EntriesSerialized
    {
      get => Entries.Any() ? JsonSerializer.Serialize(Entries) : null;
      private set
      {
        Entries.Clear();

        if (value != null)
        {
          var entries = JsonSerializer.Deserialize<Dictionary<string, string>>(value);
          if (entries != null)
          {
            foreach (var (key, entry) in entries)
            {
              Entries[key] = entry;
            }
          }
        }
      }
    }

    public void Delete(Guid userId) => ApplyChange(new DeletedEvent(userId));
    public void Update(UpdateDictionaryPayload payload, Guid userId) => ApplyChange(new UpdatedEvent(payload, userId));

    protected virtual void Apply(CreatedEvent @event)
    {
      Locale = @event.Payload.Locale;

      Apply(@event.Payload);
    }
    protected virtual void Apply(DeletedEvent @event)
    {
    }
    protected virtual void Apply(UpdatedEvent @event)
    {
      Apply(@event.Payload);
    }

    private void Apply(SaveDictionaryPayload payload)
    {
      Entries.Clear();

      if (payload.Entries != null)
      {
        foreach (EntryPayload entry in payload.Entries)
        {
          Entries[entry.Key] = entry.Value.Trim();
        }
      }
    }
  }
}
