using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record UpdateUserCommand(Guid Id, UpdateUserPayload Payload) : IRequest<User?>;
