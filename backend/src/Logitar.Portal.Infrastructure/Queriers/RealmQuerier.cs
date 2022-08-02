using Microsoft.EntityFrameworkCore;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Realms;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class RealmQuerier : IRealmQuerier
  {
    private readonly DbSet<Realm> _realms;

    public RealmQuerier(PortalDbContext dbContext)
    {
      _realms = dbContext.Realms;
    }

    public async Task<Realm?> GetAsync(string key, bool readOnly, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(key);

      return Guid.TryParse(key, out Guid id)
        ? await GetAsync(id, readOnly, cancellationToken)
        : await GetByAliasAsync(key, readOnly, cancellationToken);
    }

    public async Task<Realm?> GetAsync(Guid id, bool readOnly, CancellationToken cancellationToken)
    {
      return await _realms.ApplyTracking(readOnly)
        .Include(x => x.PasswordRecoverySenderRelation).ThenInclude(x => x!.Sender)
        .Include(x => x.PasswordRecoveryTemplateRelation).ThenInclude(x => x!.Template)
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Realm?> GetByAliasAsync(string alias, bool readOnly, CancellationToken cancellationToken)
    {
      alias = alias?.ToUpper() ?? throw new ArgumentNullException(nameof(alias));

      return await _realms.ApplyTracking(readOnly)
        .Include(x => x.PasswordRecoverySenderRelation).ThenInclude(x => x!.Sender)
        .Include(x => x.PasswordRecoveryTemplateRelation).ThenInclude(x => x!.Template)
        .SingleOrDefaultAsync(x => x.AliasNormalized == alias, cancellationToken);
    }

    public async Task<PagedList<Realm>> GetPagedAsync(string? search,
      RealmSort? sort, bool desc,
      int? index, int? count,
      bool readOnly, CancellationToken cancellationToken)
    {
      IQueryable<Realm> query = _realms.ApplyTracking(readOnly);

      if (search != null)
      {
        foreach (string term in search.Split())
        {
          if (!string.IsNullOrEmpty(term))
          {
            string pattern = $"%{term}%";

            query = query.Where(x => EF.Functions.ILike(x.Alias, pattern) || EF.Functions.ILike(x.Name, pattern));
          }
        }
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        query = sort.Value switch
        {
          RealmSort.Alias => desc ? query.OrderByDescending(x => x.Alias) : query.OrderBy(x => x.Alias),
          RealmSort.Name => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
          RealmSort.UpdatedAt => desc ? query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt) : query.OrderBy(x => x.UpdatedAt ?? x.CreatedAt),
          _ => throw new ArgumentException($"The realm sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      Realm[] realms = await query.ToArrayAsync(cancellationToken);

      return new PagedList<Realm>(realms, total);
    }
  }
}
