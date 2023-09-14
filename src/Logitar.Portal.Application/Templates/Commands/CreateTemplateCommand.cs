using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal record CreateTemplateCommand(CreateTemplatePayload Payload) : IRequest<Template>;
