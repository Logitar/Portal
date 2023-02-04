using AutoMapper;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain;
using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class RealmQuerier : IRealmQuerier
  {
    private readonly IMapper _mapper;
    private readonly DbSet<RealmEntity> _realms;

    public RealmQuerier(PortalContext context, IMapper mapper)
    {
      _mapper = mapper;
      _realms = context.Realms;
    }

    public async Task<RealmModel?> GetAsync(AggregateId id, CancellationToken cancellationToken)
      => await GetAsync(id.Value, cancellationToken);

    public async Task<RealmModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      RealmEntity? realm = await _realms.AsNoTracking()
        .Include(x => x.PasswordRecoverySender)
        .Include(x => x.PasswordRecoveryTemplate)
        .SingleOrDefaultAsync(x => x.AliasNormalized == id.ToUpper() || x.AggregateId == id, cancellationToken);

      return _mapper.Map<RealmModel>(realm);
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

            query = query.Where(x => EF.Functions.ILike(x.Alias, pattern)
              || (x.DisplayName != null && EF.Functions.ILike(x.DisplayName, pattern)));
          }
        }
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        query = sort.Value switch
        {
          RealmSort.Alias => isDescending ? query.OrderByDescending(x => x.Alias) : query.OrderBy(x => x.Alias),
          RealmSort.DisplayName => isDescending ? query.OrderByDescending(x => x.DisplayName ?? x.Alias) : query.OrderBy(x => x.DisplayName ?? x.Alias),
          RealmSort.UpdatedOn => isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn),
          _ => throw new ArgumentException($"The realm sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      RealmEntity[] realms = await query.ToArrayAsync(cancellationToken);

      return new ListModel<RealmModel>(_mapper.Map<IEnumerable<RealmModel>>(realms), total);
    }
  }
}
