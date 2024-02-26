using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Dictionaries;

public class Dictionary : Aggregate
{
  public string Locale { get; set; } // TODO(fpion): Locale

  public int EntryCount { get; set; }
  public List<DictionaryEntry> Entries { get; set; }

  public Realm? Realm { get; set; }

  public Dictionary() : this(string.Empty)
  {
  }

  public Dictionary(string locale)
  {
    Locale = locale;
    Entries = [];
  }
}
