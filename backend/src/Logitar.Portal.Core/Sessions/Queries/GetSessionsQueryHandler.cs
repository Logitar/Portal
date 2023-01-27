using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Sessions.Models;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Queries
{
  internal class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, ListModel<SessionModel>>
  {
    private readonly ISessionQuerier _sessionQuerier;

    public GetSessionsQueryHandler(ISessionQuerier sessionQuerier)
    {
      _sessionQuerier = sessionQuerier;
    }

    public async Task<ListModel<SessionModel>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
    {
      return await _sessionQuerier.GetPagedAsync(request.IsActive, request.IsPersistent, request.Realm, request.UserId,
        request.Sort, request.IsDescending,
        request.Index, request.Count,
        cancellationToken);
    }
  }
}
