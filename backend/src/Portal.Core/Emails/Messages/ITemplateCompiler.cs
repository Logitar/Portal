using Portal.Core.Emails.Templates;
using Portal.Core.Users;

namespace Portal.Core.Emails.Messages
{
  public interface ITemplateCompiler
  {
    string Compile(Template template, User? user = null, IReadOnlyDictionary<string, string?>? variables = null);
  }
}
