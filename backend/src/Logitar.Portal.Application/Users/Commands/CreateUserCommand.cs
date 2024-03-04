using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record CreateUserCommand(CreateUserPayload Payload) : ApplicationRequest, IRequest<User>;
