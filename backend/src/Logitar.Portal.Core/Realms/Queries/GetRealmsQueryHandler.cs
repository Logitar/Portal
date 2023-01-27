using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Realms.Models;
using MediatR;

namespace Logitar.Portal.Core.Realms.Queries
{
  internal class GetRealmsQueryHandler : IRequestHandler<GetRealmsQuery, ListModel<RealmModel>>
  {
    private readonly IRealmQuerier _realmQuerier;

    public GetRealmsQueryHandler(IRealmQuerier realmQuerier)
    {
      _realmQuerier = realmQuerier;
    }

    public async Task<ListModel<RealmModel>> Handle(GetRealmsQuery request, CancellationToken cancellationToken)
    {
      return await _realmQuerier.GetPagedAsync(request.Search,
        request.Sort, request.IsDescending,
        request.Index, request.Count,
        cancellationToken);
    }
  }
}
