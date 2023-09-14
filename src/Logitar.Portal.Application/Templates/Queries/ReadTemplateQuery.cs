using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Queries;

internal record ReadTemplateQuery(Guid? Id, string? Realm, string? UniqueName) : IRequest<Template?>;
