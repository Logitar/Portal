using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Core.Templates.Commands;

internal record CreateTemplate(CreateTemplateInput Input) : IRequest<Template>;
