using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Queries
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
        request.Sort, request.IsDesending,
        request.Index, request.Count,
        cancellationToken);
    }
  }
}
