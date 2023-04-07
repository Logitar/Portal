using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Realms.Queries;

internal class GetRealmsHandler : IRequestHandler<GetRealms, PagedList<Realm>>
{
  private readonly IRealmQuerier _realmQuerier;

  public GetRealmsHandler(IRealmQuerier realmQuerier)
  {
    _realmQuerier = realmQuerier;
  }

  public async Task<PagedList<Realm>> Handle(GetRealms request, CancellationToken cancellationToken)
  {
    return await _realmQuerier.GetAsync(request.Search, request.Sort, request.IsDescending,
      request.Skip, request.Limit, cancellationToken);
  }
}
