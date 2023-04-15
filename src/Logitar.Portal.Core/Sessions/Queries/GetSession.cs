using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Queries;

internal record GetSession(Guid? Id) : IRequest<Session?>;
