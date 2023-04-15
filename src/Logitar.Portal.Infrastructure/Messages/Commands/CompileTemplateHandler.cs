using Logitar.Portal.Core.Messages;
using Logitar.Portal.Core.Messages.Commands;
using MediatR;
using RazorEngine;
using RazorEngine.Templating;

namespace Logitar.Portal.Infrastructure.Messages.Commands;

internal class CompileTemplateHandler : IRequestHandler<CompileTemplate, CompiledTemplate>
{
  public Task<CompiledTemplate> Handle(CompileTemplate request, CancellationToken cancellationToken)
  {
    TemplateModel model = new(request.Dictionaries, request.User, request.Variables);
    string name = Guid.NewGuid().ToString("N");

    string value = Engine.Razor.RunCompile(request.Template.Contents, name, typeof(TemplateModel), model);

    return Task.FromResult(new CompiledTemplate(value));
  }
}
