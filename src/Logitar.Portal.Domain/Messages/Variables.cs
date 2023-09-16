namespace Logitar.Portal.Domain.Messages;

public record Variables
{
  private readonly Dictionary<string, string> _variables = new();

  public Variables()
  {
  }

  public Variables(int capacity) : this()
  {
    _variables = new(capacity);
  }

  public Variables(IEnumerable<KeyValuePair<string, string>> variables) : this(variables.Count())
  {
    foreach (KeyValuePair<string, string> variable in variables)
    {
      _variables[variable.Key] = variable.Value;
    }
  }

  public string Resolve(string key) => _variables.TryGetValue(key, out string? value) ? value : key;

  public Dictionary<string, string> AsDictionary() => new(_variables);
}
