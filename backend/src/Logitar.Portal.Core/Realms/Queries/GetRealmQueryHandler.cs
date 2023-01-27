using Logitar.Portal.Core.Realms.Models;
using MediatR;

namespace Logitar.Portal.Core.Realms.Queries
{
  internal class GetRealmQueryHandler : IRequestHandler<GetRealmQuery, RealmModel?>
  {
    private readonly IRealmQuerier _realmQuerier;

    public GetRealmQueryHandler(IRealmQuerier realmQuerier)
    {
      _realmQuerier = realmQuerier;
    }

    public async Task<RealmModel?> Handle(GetRealmQuery request, CancellationToken cancellationToken)
    {
      return await _realmQuerier.GetAsync(request.Id, cancellationToken);
    }
  }
}
