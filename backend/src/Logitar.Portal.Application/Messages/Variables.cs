using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Application.Messages;

public record Variables
{
  private readonly Dictionary<Identifier, string> _variables;

  public Variables() : this(capacity: 0)
  {
  }

  public Variables(int capacity)
  {
    _variables = new(capacity);
  }

  public Variables(IEnumerable<Variable> variables) : this(variables.Count())
  {
    foreach (Variable variable in variables)
    {
      Identifier key = new(variable.Key);
      _variables[key] = variable.Value.Trim();
    }
  }

  public IReadOnlyDictionary<Identifier, string> AsDictionary() => _variables.AsReadOnly();

  public string Resolve(string key) => _variables.TryGetValue(new Identifier(key), out string? value) ? value : key;
}
