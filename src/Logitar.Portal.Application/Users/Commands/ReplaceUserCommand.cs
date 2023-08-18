using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

public record ReplaceUserCommand(string Id, ReplaceUserPayload Payload, long? Version) : IRequest<User?>;
