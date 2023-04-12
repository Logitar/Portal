using Logitar.Portal.v2.Core.Messages;
using Logitar.Portal.v2.Core.Users;

namespace Logitar.Portal.v2.Infrastructure.Messages;

public class TemplateModel
{
  private readonly Dictionaries _dictionaries;
  private readonly IReadOnlyDictionary<string, string> _variables;

  public TemplateModel(Dictionaries? dictionaries = null, UserAggregate? user = null,
    IReadOnlyDictionary<string, string>? variables = null)
  {
    _dictionaries = dictionaries ?? new();
    User = user;
    _variables = variables ?? new Dictionary<string, string>();
  }

  public UserAggregate? User { get; }

  public string Resource(string key) => _dictionaries.GetEntry(key);
  public string? Variable(string key) => _variables.TryGetValue(key, out string? value) ? value : null;
}
