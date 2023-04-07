using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Realms;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Queriers;

internal class RealmQuerier : IRealmQuerier
{
  private readonly IMapper _mapper;
  private readonly DbSet<RealmEntity> _realms;

  public RealmQuerier(PortalContext context, IMapper mapper)
  {
    _mapper = mapper;
    _realms = context.Realms;
  }

  public async Task<Realm?> GetAsync(string idOrUniqueName, CancellationToken cancellationToken)
  {
    string aggregateId = (Guid.TryParse(idOrUniqueName, out Guid id) ? new AggregateId(id) : new(idOrUniqueName)).Value;

    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId
        || x.UniqueNameNormalized == idOrUniqueName.ToUpper(), cancellationToken);

    return _mapper.Map<Realm>(realm);
  }

  public async Task<Realm> GetAsync(RealmAggregate realm, CancellationToken cancellationToken)
  {
    RealmEntity? entity = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == realm.Id.Value, cancellationToken);

    return _mapper.Map<Realm>(entity);
  }

  public async Task<PagedList<Realm>> GetAsync(string? search, RealmSort? sort, bool isDescending,
    int? skip, int? limit, CancellationToken cancellationToken)
  {
    IQueryable<RealmEntity> query = _realms.AsNoTracking();

    if (search != null)
    {
      foreach (string term in search.Split().Where(x => !string.IsNullOrEmpty(x)))
      {
        string pattern = $"%{term}%";

        query = query.Where(x => EF.Functions.ILike(x.UniqueName, pattern)
          || EF.Functions.ILike(x.DisplayName!, pattern));
      }
    }

    long total = await query.LongCountAsync(cancellationToken);

    if (sort.HasValue)
    {
      query = sort.Value switch
      {
        RealmSort.DisplayName => isDescending ? query.OrderByDescending(x => x.DisplayName ?? x.UniqueName) : query.OrderBy(x => x.DisplayName ?? x.UniqueName),
        RealmSort.UniqueName => isDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName),
        RealmSort.UpdatedOn => isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn),
        _ => throw new ArgumentException($"The realm sort '{sort}' is not valid.", nameof(sort)),
      };
    }

    query = query.Page(skip, limit);

    RealmEntity[] realms = await query.ToArrayAsync(cancellationToken);

    return new PagedList<Realm>
    {
      Items = _mapper.Map<IEnumerable<Realm>>(realms),
      Total = total
    };
  }
}
