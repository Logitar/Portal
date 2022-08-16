using Logitar.Portal.Application;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Core.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class DictionaryQuerier : IDictionaryQuerier
  {
    private readonly DbSet<Dictionary> _dictionaries;

    public DictionaryQuerier(PortalDbContext dbContext)
    {
      _dictionaries = dbContext.Dictionaries;
    }

    public async Task<Dictionary?> GetAsync(Guid id, bool readOnly, CancellationToken cancellationToken)
    {
      return await _dictionaries.ApplyTracking(readOnly)
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PagedList<Dictionary>> GetPagedAsync(string? locale, string? realm,
      DictionarySort? sort, bool desc,
      int? index, int? count,
      bool readOnly, CancellationToken cancellationToken)
    {
      IQueryable<Dictionary> query = _dictionaries.ApplyTracking(readOnly)
        .Include(x => x.Realm);

      if (realm == null)
      {
        query = query.Where(x => x.RealmSid == null);
      }
      else
      {
        query = Guid.TryParse(realm, out Guid realmId)
          ? query.Where(x => x.Realm != null && x.Realm.Id == realmId)
          : query.Where(x => x.Realm != null && x.Realm.AliasNormalized == realm.ToUpper());
      }

      if (locale != null)
      {
        query = query.Where(x => x.Locale == locale);
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        query = sort.Value switch
        {
          DictionarySort.RealmLocale => desc
            ? query.OrderByDescending(x => x.Realm == null ? null : x.Realm.Name).ThenByDescending(x => x.Locale)
            : query.OrderBy(x => x.Realm == null ? null : x.Realm.Name).ThenBy(x => x.Locale),
          DictionarySort.UpdatedAt => desc ? query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt) : query.OrderBy(x => x.UpdatedAt ?? x.CreatedAt),
          _ => throw new ArgumentException($"The dictionary sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      Dictionary[] dictionaries = await query.ToArrayAsync(cancellationToken);

      return new PagedList<Dictionary>(dictionaries, total);
    }
  }
}
