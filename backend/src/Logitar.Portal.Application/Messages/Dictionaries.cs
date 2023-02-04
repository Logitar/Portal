using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Application.Messages
{
  public class Dictionaries
  {
    public Dictionaries(Dictionary? @default = null, Dictionary? fallback = null, Dictionary? preferred = null)
    {
      Default = @default;
      Fallback = fallback;
      Preferred = preferred;
    }

    public Dictionary? Default { get; }
    public Dictionary? Fallback { get; }
    public Dictionary? Preferred { get; }

    public string GetEntry(string key)
    {
      if (Preferred?.Entries.TryGetValue(key, out string? value) == true
        || Fallback?.Entries.TryGetValue(key, out value) == true
        || Default?.Entries.TryGetValue(key, out value) == true)
      {
        return value;
      }

      return key;
    }
  }
}
