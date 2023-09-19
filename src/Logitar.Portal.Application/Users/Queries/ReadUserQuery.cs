using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries;

internal record ReadUserQuery(Guid? Id, string? Realm, string? UniqueName, string? IdentifierKey, string? IdentifierValue) : IRequest<User?>;
