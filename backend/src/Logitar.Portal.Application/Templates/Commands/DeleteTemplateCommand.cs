using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal record DeleteTemplateCommand(Guid Id) : Activity, IRequest<Template?>;
