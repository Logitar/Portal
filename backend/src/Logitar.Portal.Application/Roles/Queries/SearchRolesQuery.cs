using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.Roles.Queries;

internal record SearchRolesQuery(SearchRolesPayload Payload) : Activity, IRequest<SearchResults<Role>>;
