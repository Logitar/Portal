using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Queries;

internal class GetSessionsHandler : IRequestHandler<GetSessions, PagedList<Session>>
{
  private readonly ISessionQuerier _sessionQuerier;

  public GetSessionsHandler(ISessionQuerier sessionQuerier)
  {
    _sessionQuerier = sessionQuerier;
  }

  public async Task<PagedList<Session>> Handle(GetSessions request, CancellationToken cancellationToken)
  {
    return await _sessionQuerier.GetAsync(request.IsActive, request.IsPersistent, request.Realm, request.UserId,
      request.Sort, request.IsDescending, request.Skip, request.Limit, cancellationToken);
  }
}
