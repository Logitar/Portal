using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Queries;

internal record SearchRolesQuery(SearchRolesPayload Payload) : IRequest<SearchResults<Role>>;
