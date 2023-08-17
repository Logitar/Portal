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
    Dictionary<string, Realm> realms = new(capacity: 2);

    if (query.Id != null)
    {
      Realm? realm = await _realmQuerier.ReadAsync(query.Id, cancellationToken);
      if (realm != null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (query.UniqueSlug != null)
    {
      Realm? realm = await _realmQuerier.ReadByUniqueSlugAsync(query.UniqueSlug, cancellationToken);
      if (realm != null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (realms.Count > 1)
    {
      throw new TooManyResultsException<Realm>(expected: 1, actual: realms.Count);
    }

    return realms.Values.SingleOrDefault();
  }
}
