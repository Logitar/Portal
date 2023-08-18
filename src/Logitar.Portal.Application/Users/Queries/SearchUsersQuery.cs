using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries;

public record SearchUsersQuery(SearchUsersPayload Payload) : IRequest<SearchResults<User>>;
