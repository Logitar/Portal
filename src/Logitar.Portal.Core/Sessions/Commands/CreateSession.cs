using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands;

internal record CreateSession(CreateSessionInput Input) : IRequest<Session>;
