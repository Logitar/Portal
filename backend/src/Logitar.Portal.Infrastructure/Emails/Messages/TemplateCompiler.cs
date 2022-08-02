using Logitar.Portal.Core.Emails.Messages;
using Logitar.Portal.Core.Emails.Templates;
using Logitar.Portal.Core.Users;
using RazorEngine;
using RazorEngine.Templating;

namespace Logitar.Portal.Infrastructure.Emails.Messages
{
  internal class TemplateCompiler : ITemplateCompiler
  {
    public string Compile(Template template, User? user = null, IReadOnlyDictionary<string, string?>? variables = null)
    {
      ArgumentNullException.ThrowIfNull(template);

      var model = new TemplateModel(user, variables);
      string name = Guid.NewGuid().ToString("N");

      return Engine.Razor.RunCompile(template.Contents, name, typeof(TemplateModel), model);
    }
  }
}
