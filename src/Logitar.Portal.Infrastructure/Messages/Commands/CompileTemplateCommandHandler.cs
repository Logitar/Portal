using Logitar.Portal.Application.Messages;
using Logitar.Portal.Application.Messages.Commands;
using MediatR;
using RazorEngine;
using RazorEngine.Templating;

namespace Logitar.Portal.Infrastructure.Messages.Commands;

internal class CompileTemplateCommandHandler : IRequestHandler<CompileTemplateCommand, CompiledTemplate>
{
  public Task<CompiledTemplate> Handle(CompileTemplateCommand command, CancellationToken cancellationToken)
  {
    TemplateModel model = new(command.Dictionaries, command.User, command.Variables);
    string name = Guid.NewGuid().ToString("N");

    string value = Engine.Razor.RunCompile(command.Template.Contents, name, typeof(TemplateModel), model);

    return Task.FromResult(new CompiledTemplate(value));
  }
}
