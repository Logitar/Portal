using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries;

public record ReadUserQuery(Guid? Id, string? UniqueName, CustomIdentifier? Identifier) : ApplicationRequest, IRequest<User?>;
