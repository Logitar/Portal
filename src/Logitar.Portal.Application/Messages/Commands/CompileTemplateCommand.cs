using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands;

public record CompileTemplateCommand : IRequest<CompiledTemplate>
{
  public CompileTemplateCommand(TemplateAggregate template, Dictionaries? dictionaries = null,
    UserAggregate? user = null, Variables? variables = null)
  {
    Dictionaries = dictionaries ?? new();
    Template = template;
    User = user;
    Variables = variables ?? new();
  }

  public Dictionaries Dictionaries { get; }
  public TemplateAggregate Template { get; }
  public UserAggregate? User { get; }
  public Variables Variables { get; }
}
