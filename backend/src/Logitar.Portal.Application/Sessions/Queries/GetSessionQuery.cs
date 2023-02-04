using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Queries
{
  internal record GetSessionQuery(string Id) : IRequest<SessionModel?>;
}
