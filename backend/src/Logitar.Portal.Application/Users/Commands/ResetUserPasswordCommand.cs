using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record ResetUserPasswordCommand(string Id, ResetUserPasswordPayload Payload) : IRequest<User?>;
