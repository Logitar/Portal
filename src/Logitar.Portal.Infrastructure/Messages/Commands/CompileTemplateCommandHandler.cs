using Logitar.Portal.Application.Messages;
using Logitar.Portal.Application.Messages.Commands;
using MediatR;

namespace Logitar.Portal.Infrastructure.Messages.Commands;

internal class CompileTemplateCommandHandler : IRequestHandler<CompileTemplateCommand, CompiledTemplate>
{
  public CompileTemplateCommandHandler()
  {
  }

  public Task<CompiledTemplate> Handle(CompileTemplateCommand command, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
