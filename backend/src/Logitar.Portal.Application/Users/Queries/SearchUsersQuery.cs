using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries;

internal record SearchUsersQuery(SearchUsersPayload Payload) : Activity, IRequest<SearchResults<User>>;
