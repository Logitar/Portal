using Logitar.Portal.Application.Messages;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using RazorEngine;
using RazorEngine.Templating;

namespace Logitar.Portal.Infrastructure.Messages
{
  internal class TemplateCompiler : ITemplateCompiler
  {
    public string Compile(Template template, Dictionaries? dictionaries, User? user, IReadOnlyDictionary<string, string?>? variables)
    {
      string name = AggregateId.NewId().Value;
      ContentModel model = new(dictionaries, user, variables);

      return Engine.Razor.RunCompile(template.Contents, name, typeof(ContentModel), model);
    }
  }
}
