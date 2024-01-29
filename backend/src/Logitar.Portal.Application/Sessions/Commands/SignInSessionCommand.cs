using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal record SignInSessionCommand(SignInSessionPayload Payload) : IRequest<Session>;
