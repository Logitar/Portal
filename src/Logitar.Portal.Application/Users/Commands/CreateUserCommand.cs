using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

public record CreateUserCommand(CreateUserPayload Payload) : IRequest<User>;
