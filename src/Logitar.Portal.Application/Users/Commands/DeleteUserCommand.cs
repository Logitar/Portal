using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

public record DeleteUserCommand(string Id) : IRequest<User?>;
