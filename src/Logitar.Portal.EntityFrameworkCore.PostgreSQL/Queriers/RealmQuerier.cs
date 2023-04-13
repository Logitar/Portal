using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Queriers;

internal class RealmQuerier : IRealmQuerier
{
  private readonly IMapper _mapper;
  private readonly DbSet<RealmEntity> _realms;

  public RealmQuerier(PortalContext context, IMapper mapper)
  {
    _mapper = mapper;
    _realms = context.Realms;
  }

  public async Task<Realm> GetAsync(RealmAggregate realm, CancellationToken cancellationToken)
  {
    RealmEntity entity = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == realm.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The realm entity '{realm.Id}' could not be found.");

    return _mapper.Map<Realm>(entity);
  }

  public async Task<Realm?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return _mapper.Map<Realm>(realm);
  }

  public async Task<Realm?> GetAsync(string uniqueName, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueName.ToUpper(), cancellationToken);

    return _mapper.Map<Realm>(realm);
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
