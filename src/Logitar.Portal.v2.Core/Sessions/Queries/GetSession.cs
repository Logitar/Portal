using Logitar.Portal.v2.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.v2.Core.Sessions.Queries;

internal record GetSession(Guid? Id) : IRequest<Session?>;
