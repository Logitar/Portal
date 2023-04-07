using Logitar.Portal.v2.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Realms.Queries;

internal class GetRealmHandler : IRequestHandler<GetRealm, Realm?>
{
  private readonly IRealmQuerier _realmQuerier;

  public GetRealmHandler(IRealmQuerier realmQuerier)
  {
    _realmQuerier = realmQuerier;
  }

  public async Task<Realm?> Handle(GetRealm request, CancellationToken cancellationToken)
  {
    return await _realmQuerier.GetAsync(request.IdOrUniqueName, cancellationToken);
  }
}
