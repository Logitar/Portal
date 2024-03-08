using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

public record RenewSessionCommand(RenewSessionPayload Payload) : Activity, IRequest<Session>;
