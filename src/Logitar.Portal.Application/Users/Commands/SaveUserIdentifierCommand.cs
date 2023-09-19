using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record SaveUserIdentifierCommand(Guid Id, SaveIdentifierPayload Payload) : IRequest<User?>;
