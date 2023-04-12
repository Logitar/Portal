using Logitar.Portal.Domain.Emails.Templates;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Emails.Messages
{
  public interface ITemplateCompiler
  {
    string Compile(
      Template template,
      Dictionaries? dictionaries = null,
      User? user = null,
      IReadOnlyDictionary<string, string?>? variables = null
    );
  }
}
