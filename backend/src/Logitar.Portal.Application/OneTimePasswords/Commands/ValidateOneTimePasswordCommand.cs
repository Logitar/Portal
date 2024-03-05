using Logitar.Portal.Contracts.Passwords;
using MediatR;

namespace Logitar.Portal.Application.OneTimePasswords.Commands;

internal record ValidateOneTimePasswordCommand(Guid Id, ValidateOneTimePasswordPayload Payload) : ApplicationRequest, IRequest<OneTimePassword?>;
