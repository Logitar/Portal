using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal record UpdateTemplateCommand(Guid Id, UpdateTemplatePayload Payload) : IRequest<Template?>;
