using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Queries;

internal class ReadRealmQueryHandler : IRequestHandler<ReadRealmQuery, RealmModel?>
{
  private readonly IRealmQuerier _realmQuerier;

  public ReadRealmQueryHandler(IRealmQuerier realmQuerier)
  {
    _realmQuerier = realmQuerier;
  }

  public async Task<RealmModel?> Handle(ReadRealmQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, RealmModel> realms = new(capacity: 2);

    if (query.Id.HasValue)
    {
      RealmModel? realm = await _realmQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (realm != null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueSlug))
    {
      RealmModel? realm = await _realmQuerier.ReadAsync(query.UniqueSlug, cancellationToken);
      if (realm != null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (realms.Count > 1)
    {
      throw new TooManyResultsException<RealmModel>(expectedCount: 1, actualCount: realms.Count);
    }

    return realms.Values.SingleOrDefault();
  }
}
