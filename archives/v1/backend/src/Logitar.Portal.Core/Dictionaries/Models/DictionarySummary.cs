namespace Logitar.Portal.Core.Dictionaries.Models
{
  public class DictionarySummary : AggregateSummary
  {
    public string? Realm { get; set; }

    public string Locale { get; set; } = null!;

    public int Entries { get; set; }
  }
}
