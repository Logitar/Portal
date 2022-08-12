using Logitar.Portal.Core.Emails.Templates;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Emails.Messages
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
