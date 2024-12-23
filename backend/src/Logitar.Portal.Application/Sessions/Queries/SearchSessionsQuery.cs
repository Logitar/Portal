using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Queries;

internal record SearchSessionsQuery(SearchSessionsPayload Payload) : Activity, IRequest<SearchResults<SessionModel>>;

internal class SearchSessionsQueryHandler : IRequestHandler<SearchSessionsQuery, SearchResults<SessionModel>>
{
  private readonly ISessionQuerier _sessionQuerier;

  public SearchSessionsQueryHandler(ISessionQuerier sessionQuerier)
  {
    _sessionQuerier = sessionQuerier;
  }

  public async Task<SearchResults<SessionModel>> Handle(SearchSessionsQuery query, CancellationToken cancellationToken)
  {
    return await _sessionQuerier.SearchAsync(query.Realm, query.Payload, cancellationToken);
  }
}
