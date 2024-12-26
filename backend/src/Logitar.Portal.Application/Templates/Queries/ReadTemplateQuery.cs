using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Queries;

internal record ReadTemplateQuery(Guid? Id, string? UniqueKey) : Activity, IRequest<TemplateModel?>;
