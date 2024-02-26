using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Dictionaries;

public class Dictionary : Aggregate
{
  public Locale Locale { get; set; }

  public int EntryCount { get; set; }
  public List<DictionaryEntry> Entries { get; set; }

  public Realm? Realm { get; set; }

  public Dictionary() : this(new Locale())
  {
  }

  public Dictionary(Locale locale)
  {
    Locale = locale;
    Entries = [];
  }
}
