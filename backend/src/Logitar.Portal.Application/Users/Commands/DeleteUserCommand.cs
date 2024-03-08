using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record DeleteUserCommand(Guid Id) : Activity, IRequest<User?>;
