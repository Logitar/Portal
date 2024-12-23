using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Dictionaries;

public class DictionaryModel : Aggregate
{
  public Locale Locale { get; set; }

  public int EntryCount { get; set; }
  public List<DictionaryEntry> Entries { get; set; }

  public Realm? Realm { get; set; }

  public DictionaryModel() : this(new Locale())
  {
  }

  public DictionaryModel(Locale locale)
  {
    Locale = locale;
    Entries = [];
  }
}
