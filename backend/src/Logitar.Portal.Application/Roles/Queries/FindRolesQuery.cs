using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Portal.Application.Roles.Queries;

internal record FindRolesQuery(TenantId? TenantId, IEnumerable<string> Roles, string? PropertyName = null) : IRequest<IEnumerable<RoleAggregate>>;
