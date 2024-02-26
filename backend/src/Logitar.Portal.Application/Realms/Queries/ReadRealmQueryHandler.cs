using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Queries;

internal class ReadRealmQueryHandler : IRequestHandler<ReadRealmQuery, Realm?>
{
  private readonly IRealmQuerier _realmQuerier;

  public ReadRealmQueryHandler(IRealmQuerier realmQuerier)
  {
    _realmQuerier = realmQuerier;
  }

  public async Task<Realm?> Handle(ReadRealmQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Realm> realms = new(capacity: 2);

    if (query.Id.HasValue)
    {
      Realm? realm = await _realmQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (realm != null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueSlug))
    {
      Realm? realm = await _realmQuerier.ReadAsync(query.UniqueSlug, cancellationToken);
      if (realm != null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (realms.Count > 1)
    {
      throw new TooManyResultsException<Realm>(expectedCount: 1, actualCount: realms.Count);
    }

    return realms.Values.SingleOrDefault();
  }
}
