using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Queries;

internal record ReadRoleQuery(string? Id, string? UniqueName) : IRequest<Role?>;
