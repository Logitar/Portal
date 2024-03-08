using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Queries;

internal record ReadRoleQuery(Guid? Id, string? UniqueName) : Activity, IRequest<Role?>;
