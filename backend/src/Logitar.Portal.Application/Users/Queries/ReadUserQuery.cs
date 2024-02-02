using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries;

internal record ReadUserQuery(Guid? Id, string? UniqueName, CustomIdentifier? Identifier) : IRequest<User?>;
