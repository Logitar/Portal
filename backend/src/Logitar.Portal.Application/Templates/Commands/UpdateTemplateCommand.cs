using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands
{
  internal record UpdateTemplateCommand(string Id, UpdateTemplatePayload Payload) : IRequest<TemplateModel>;
}
