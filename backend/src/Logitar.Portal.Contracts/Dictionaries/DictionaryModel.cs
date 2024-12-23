using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Dictionaries;

public class DictionaryModel : AggregateModel
{
  public LocaleModel Locale { get; set; }

  public int EntryCount { get; set; }
  public List<DictionaryEntry> Entries { get; set; }

  public RealmModel? Realm { get; set; }

  public DictionaryModel() : this(new LocaleModel())
  {
  }

  public DictionaryModel(LocaleModel locale)
  {
    Locale = locale;
    Entries = [];
  }
}
