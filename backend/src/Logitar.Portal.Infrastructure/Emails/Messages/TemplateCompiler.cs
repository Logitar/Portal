using Logitar.Portal.Core.Emails.Messages;
using Logitar.Portal.Core.Emails.Templates;
using Logitar.Portal.Core.Users;
using RazorEngine;
using RazorEngine.Templating;

namespace Logitar.Portal.Infrastructure.Emails.Messages
{
  internal class TemplateCompiler : ITemplateCompiler
  {
    public string Compile(Template template, Dictionaries? dictionaries, User? user, IReadOnlyDictionary<string, string?>? variables)
    {
      ArgumentNullException.ThrowIfNull(template);

      var model = new TemplateModel(dictionaries, user, variables);
      string name = Guid.NewGuid().ToString("N");

      return Engine.Razor.RunCompile(template.Contents, name, typeof(TemplateModel), model);
    }
  }
}
