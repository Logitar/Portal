using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Queries;

internal record GetSessions(bool? IsActive, bool? IsPersistent, string? Realm, Guid? UserId,
  SessionSort? Sort, bool IsDescending, int? Skip, int? Limit) : IRequest<PagedList<Session>>;
