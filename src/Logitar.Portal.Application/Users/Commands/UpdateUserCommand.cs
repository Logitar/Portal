using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

public record UpdateUserCommand(string Id, UpdateUserPayload Payload) : IRequest<User?>;
