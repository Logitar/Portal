using Microsoft.EntityFrameworkCore;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Emails.Templates;
using Logitar.Portal.Core.Realms;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class TemplateQuerier : ITemplateQuerier
  {
    private readonly DbSet<Template> _templates;

    public TemplateQuerier(PortalDbContext dbContext)
    {
      _templates = dbContext.Templates;
    }

    public async Task<Template?> GetAsync(string key, Realm? realm, bool readOnly, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(key);

      return Guid.TryParse(key, out Guid id)
        ? await GetAsync(id, readOnly, cancellationToken)
        : await GetByKeyAsync(key, realm, readOnly, cancellationToken);
    }

    public async Task<Template?> GetAsync(Guid id, bool readOnly, CancellationToken cancellationToken)
    {
      return await _templates.ApplyTracking(readOnly)
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Template?> GetByKeyAsync(string key, Realm? realm, bool readOnly, CancellationToken cancellationToken)
    {
      key = key?.ToUpper() ?? throw new ArgumentNullException(nameof(key));

      IQueryable<Template> query = _templates.ApplyTracking(readOnly).Include(x => x.Realm);

      return realm == null
        ? await query.SingleOrDefaultAsync(x => x.RealmSid == null && x.KeyNormalized == key, cancellationToken)
        : await query.SingleOrDefaultAsync(x => x.RealmSid == realm.Sid && x.KeyNormalized == key, cancellationToken);
    }

    public async Task<PagedList<Template>> GetPagedAsync(string? realm, string? search,
      TemplateSort? sort, bool desc,
      int? index, int? count,
      bool readOnly, CancellationToken cancellationToken)
    {
      IQueryable<Template> query = _templates.ApplyTracking(readOnly)
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

      if (search != null)
      {
        foreach (string term in search.Split())
        {
          if (!string.IsNullOrEmpty(term))
          {
            string pattern = $"%{term}%";

            query = query.Where(x => EF.Functions.ILike(x.Key, pattern)
              || (x.DisplayName != null && EF.Functions.ILike(x.DisplayName, pattern)));
          }
        }
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        query = sort.Value switch
        {
          TemplateSort.DisplayName => desc ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName),
          TemplateSort.Key => desc ? query.OrderByDescending(x => x.Key) : query.OrderBy(x => x.Key),
          TemplateSort.UpdatedAt => desc ? query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt) : query.OrderBy(x => x.UpdatedAt ?? x.CreatedAt),
          _ => throw new ArgumentException($"The template sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      Template[] templates = await query.ToArrayAsync(cancellationToken);

      return new PagedList<Template>(templates, total);
    }
  }
}
