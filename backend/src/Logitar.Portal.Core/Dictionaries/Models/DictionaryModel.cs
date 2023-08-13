using Logitar.Portal.Core.Realms.Models;

namespace Logitar.Portal.Core.Dictionaries.Models
{
  public class DictionaryModel : AggregateModel
  {
    public RealmModel? Realm { get; set; }

    public string Locale { get; set; } = null!;

    public IEnumerable<EntryModel> Entries { get; set; } = null!;
  }
}
