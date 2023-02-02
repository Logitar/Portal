using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Queries
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
        request.Sort, request.IsDecending,
        request.Index, request.Count,
        cancellationToken);
    }
  }
}
