using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal record ReplaceTemplateCommand(Guid Id, ReplaceTemplatePayload Payload, long? Version) : Activity, IRequest<Template?>;
