using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries;

public record ReadUserQuery(Guid? Id, string? UniqueName, CustomIdentifierModel? Identifier) : Activity, IRequest<UserModel?>;
