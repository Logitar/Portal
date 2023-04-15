using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Core.Templates.Commands;

internal record UpdateTemplate(Guid Id, UpdateTemplateInput Input) : IRequest<Template>;
