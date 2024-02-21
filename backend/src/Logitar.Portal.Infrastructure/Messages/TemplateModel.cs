using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Messages;

namespace Logitar.Portal.Infrastructure.Messages;

public class TemplateModel
{
  private readonly Dictionaries _dictionaries;
  private readonly Variables _variables;

  public TemplateModel(Dictionaries? dictionaries = null, LocaleUnit? locale = null, UserAggregate? user = null, Variables? variables = null)
  {
    _dictionaries = dictionaries ?? new();
    _variables = variables ?? new();

    Locale = locale;
    User = user;
  }

  public LocaleUnit? Locale { get; }
  public UserAggregate? User { get; }

  public string Resource(string key) => _dictionaries.Translate(key);
  public string Variable(string key) => _variables.Resolve(key);
}
