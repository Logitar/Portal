using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Queries;

internal class SearchRealmsQueryHandler : IRequestHandler<SearchRealmsQuery, SearchResults<Realm>>
{
  private readonly IRealmQuerier _realmQuerier;

  public SearchRealmsQueryHandler(IRealmQuerier realmQuerier)
  {
    _realmQuerier = realmQuerier;
  }

  public async Task<SearchResults<Realm>> Handle(SearchRealmsQuery query, CancellationToken cancellationToken)
  {
    return await _realmQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
