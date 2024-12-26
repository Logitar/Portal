using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Domain.Templates;
using MediatR;
using RazorEngine;
using RazorEngine.Templating;

namespace Logitar.Portal.Infrastructure.Messages.Commands;

internal class CompileTemplateCommandHandler : IRequestHandler<CompileTemplateCommand, Content>
{
  public Task<Content> Handle(CompileTemplateCommand command, CancellationToken cancellationToken)
  {
    TemplateModel model = new(command.Dictionaries, command.Locale, command.User, command.Variables);
    string name = string.Join('_', command.Template.UniqueKey.Value, command.MessageId.Value);

    Content content = command.Template.Content;
    string text = Engine.Razor.RunCompile(content.Text, name, typeof(TemplateModel), model);

    return Task.FromResult(content.Create(text));
  }
}
