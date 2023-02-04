using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Queries
{
  internal record GetSessionsQuery(bool? IsActive, bool? IsPersistent, string? Realm, string? UserId,
    SessionSort? Sort, bool IsDesending, int? Index, int? Count) : IRequest<ListModel<SessionModel>>;
}
