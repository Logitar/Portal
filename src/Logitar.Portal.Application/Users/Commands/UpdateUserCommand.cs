using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record UpdateUserCommand(string Id, UpdateUserPayload Payload) : IRequest<User?>;
