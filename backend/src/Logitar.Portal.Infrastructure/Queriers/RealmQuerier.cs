using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class RealmQuerier : IRealmQuerier
  {
    private readonly IMappingService _mapper;
    private readonly DbSet<RealmEntity> _realms;

    public RealmQuerier(PortalContext context, IMappingService mapper)
    {
      _mapper = mapper;
      _realms = context.Realms;
    }

    public async Task<RealmModel?> GetAsync(string idOrAlias, CancellationToken cancellationToken)
    {
      RealmEntity? realm = await _realms.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == idOrAlias || x.AliasNormalized == idOrAlias.ToUpper(), cancellationToken);

      return realm == null ? null : await _mapper.MapAsync<RealmModel>(realm, cancellationToken);
    }

    public async Task<ListModel<RealmModel>> GetPagedAsync(string? search,
      RealmSort? sort, bool isDescending,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      IQueryable<RealmEntity> query = _realms.AsNoTracking();

      if (search != null)
      {
        foreach (string term in search.Split())
        {
          if (!string.IsNullOrEmpty(term))
          {
            string pattern = $"%{term}%";

            query = query.Where(x => EF.Functions.ILike(x.Alias, pattern) || EF.Functions.ILike(x.DisplayName, pattern));
          }
        }
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        query = sort.Value switch
        {
          RealmSort.Alias => isDescending ? query.OrderByDescending(x => x.Alias) : query.OrderBy(x => x.Alias),
          RealmSort.DisplayName => isDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName),
          RealmSort.UpdatedOn => isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn),
          _ => throw new ArgumentException($"The realm sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      RealmEntity[] realms = await query.ToArrayAsync(cancellationToken);

      return new ListModel<RealmModel>(await _mapper.MapAsync<RealmModel>(realms, cancellationToken), total);
    }
  }
}
