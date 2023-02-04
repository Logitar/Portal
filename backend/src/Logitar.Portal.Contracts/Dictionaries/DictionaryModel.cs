using Logitar.Portal.Contracts.Realms;
using System.Globalization;

namespace Logitar.Portal.Contracts.Dictionaries
{
  public record DictionaryModel : AggregateModel
  {
    public RealmModel? Realm { get; set; }

    public CultureInfo Locale { get; set; } = CultureInfo.InvariantCulture;

    public IEnumerable<EntryModel> Entries { get; set; } = Enumerable.Empty<EntryModel>();
  }
}
