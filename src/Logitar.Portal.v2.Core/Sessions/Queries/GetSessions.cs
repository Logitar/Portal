using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.v2.Core.Sessions.Queries;

internal record GetSessions(bool? IsActive, bool? IsPersistent, string? Realm, Guid? UserId,
  SessionSort? Sort, bool IsDescending, int? Skip, int? Limit) : IRequest<PagedList<Session>>;
