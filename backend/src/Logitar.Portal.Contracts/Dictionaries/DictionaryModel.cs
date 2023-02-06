using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Dictionaries
{
  public record DictionaryModel : AggregateModel
  {
    public RealmModel? Realm { get; set; }

    public string Locale { get; set; } = string.Empty;

    public IEnumerable<EntryModel> Entries { get; set; } = Enumerable.Empty<EntryModel>();
  }
}
