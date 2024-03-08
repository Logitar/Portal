using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal record CreateSessionCommand(CreateSessionPayload Payload) : Activity, IRequest<Session>;
