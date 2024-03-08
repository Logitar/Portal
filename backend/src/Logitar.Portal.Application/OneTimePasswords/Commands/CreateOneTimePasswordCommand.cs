using Logitar.Portal.Contracts.Passwords;
using MediatR;

namespace Logitar.Portal.Application.OneTimePasswords.Commands;

internal record CreateOneTimePasswordCommand(CreateOneTimePasswordPayload Payload) : Activity,IRequest<OneTimePassword>;
