using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries;

internal record SearchUsersQuery(SearchUsersPayload Payload) : IRequest<SearchResults<User>>;
