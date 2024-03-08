using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record SaveUserIdentifierCommand(Guid Id, string Key, SaveUserIdentifierPayload Payload) : Activity, IRequest<User?>;
