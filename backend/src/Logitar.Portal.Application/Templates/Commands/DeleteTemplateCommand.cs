using MediatR;

namespace Logitar.Portal.Application.Templates.Commands
{
  internal record DeleteTemplateCommand(string Id) : IRequest;
}
