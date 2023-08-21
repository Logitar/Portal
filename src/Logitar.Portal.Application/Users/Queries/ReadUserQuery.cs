using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries;

internal record ReadUserQuery(string? Id, string? Realm, string? UniqueName) : IRequest<User?>;
