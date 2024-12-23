using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Dictionaries;

public class Dictionary : Aggregate
{
  public LocaleModel Locale { get; set; }

  public int EntryCount { get; set; }
  public List<DictionaryEntry> Entries { get; set; }

  public Realm? Realm { get; set; }

  public Dictionary() : this(new LocaleModel())
  {
  }

  public Dictionary(LocaleModel locale)
  {
    Locale = locale;
    Entries = [];
  }
}
