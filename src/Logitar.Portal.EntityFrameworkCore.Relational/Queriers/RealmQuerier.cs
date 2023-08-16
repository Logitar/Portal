using AutoMapper;
using Logitar.Data;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class RealmQuerier : IRealmQuerier
{
  private readonly IMapper _mapper;
  private readonly DbSet<RealmEntity> _realms;
  private readonly IPortalSqlHelper _sql;

  public RealmQuerier(PortalContext context, IMapper mapper, IPortalSqlHelper sql)
  {
    _mapper = mapper;
    _realms = context.Realms;
    _sql = sql;
  }

  public async Task<Realm> ReadAsync(RealmAggregate realm, CancellationToken cancellationToken)
    => await ReadAsync(realm.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The realm entity 'AggregateId={realm.Id}' could not be found.");
  public async Task<Realm?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

    return _mapper.Map<Realm?>(realm);
  }

  public async Task<Realm?> ReadByUniqueSlugAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = uniqueSlug.ToUpper();

    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueSlugNormalized == uniqueSlugNormalized, cancellationToken);

    return _mapper.Map<Realm?>(realm);
  }

  public async Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sql.QueryFrom(PortalDb.Realms.Table).SelectAll(PortalDb.Realms.Table);
    _sql.ApplyTextSearch(builder, payload.Id, PortalDb.Realms.AggregateId);
    _sql.ApplyTextSearch(builder, payload.Search, PortalDb.Realms.UniqueSlug, PortalDb.Realms.DisplayName);

    IQueryable<RealmEntity> query = _realms.FromQuery(builder.Build()).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    if (payload.Sort.Any())
    {
      IOrderedQueryable<RealmEntity>? ordered = null;

      foreach (RealmSortOption sort in payload.Sort)
      {
        switch (sort.Field)
        {
          case RealmSort.DisplayName:
            ordered = (ordered == null)
              ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
              : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
            break;
          case RealmSort.UniqueSlug:
            ordered = (ordered == null)
              ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueSlug) : query.OrderBy(x => x.UniqueSlug))
              : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueSlug) : ordered.ThenBy(x => x.UniqueSlug));
            break;
          case RealmSort.UpdatedOn:
            ordered = (ordered == null)
              ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
              : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
            break;
        }
      }
    }

    query = query.ApplyPaging(payload);

    RealmEntity[] realms = await query.ToArrayAsync(cancellationToken);

    return new SearchResults<Realm>(_mapper.Map<IEnumerable<Realm>>(realms), total);
  }
}
