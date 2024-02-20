using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Messages;

namespace Logitar.Portal.Infrastructure.Messages;

public class TemplateModel
{
  private readonly Dictionaries _dictionaries;
  private readonly Variables _variables;

  public TemplateModel(Dictionaries? dictionaries = null, UserAggregate? user = null, Variables? variables = null)
  {
    _dictionaries = dictionaries ?? new();
    User = user;
    _variables = variables ?? new();
  }

  public UserAggregate? User { get; }

  public string Resource(string key) => _dictionaries.Translate(key);
  public string Variable(string key) => _variables.Resolve(key);
}
