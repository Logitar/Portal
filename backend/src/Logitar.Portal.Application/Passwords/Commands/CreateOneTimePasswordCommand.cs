using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Passwords;
using MediatR;

namespace Logitar.Portal.Application.Passwords.Commands;

internal record CreateOneTimePasswordCommand(CreateOneTimePasswordPayload Payload) : Activity, IRequest<OneTimePasswordModel>;
