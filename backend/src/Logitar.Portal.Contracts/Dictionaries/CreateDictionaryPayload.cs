using System.Globalization;

namespace Logitar.Portal.Contracts.Dictionaries
{
  public record CreateDictionaryPayload
  {
    public string? Realm { get; set; }
    public CultureInfo Locale { get; set; } = CultureInfo.InvariantCulture;

    public IEnumerable<EntryPayload>? Entries { get; set; }
  }
}
