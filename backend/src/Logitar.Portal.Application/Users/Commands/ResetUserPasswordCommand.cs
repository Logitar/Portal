using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record ResetUserPasswordCommand(Guid Id, ResetUserPasswordPayload Payload) : ApplicationRequest, IRequest<User?>;
