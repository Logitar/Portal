using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Dictionaries;

public record Dictionary : Aggregate
{
  public Guid Id { get; set; }

  public string Locale { get; set; } = string.Empty;

  public IEnumerable<Entry> Entries { get; set; } = Enumerable.Empty<Entry>();

  public Realm Realm { get; set; } = new();
}
