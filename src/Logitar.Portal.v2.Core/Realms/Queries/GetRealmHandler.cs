using Logitar.EventSourcing;
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
    List<Realm> realms = new(capacity: 2);

    if (request.Id.HasValue)
    {
      realms.AddIfNotNull(await _realmQuerier.GetAsync(request.Id.Value, cancellationToken));
    }
    if (request.UniqueName != null)
    {
      realms.AddIfNotNull(await _realmQuerier.GetAsync(request.UniqueName, cancellationToken));
    }

    if (realms.Count > 1)
    {
      throw new TooManyResultsException();
    }

    return realms.SingleOrDefault();
  }
}
