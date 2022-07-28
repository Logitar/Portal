using Microsoft.EntityFrameworkCore;
using Portal.Core;
using Portal.Core.Realms;
using Portal.Core.Templates;

namespace Portal.Infrastructure.Queriers
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
      key = key?.ToUpper() ?? throw new ArgumentNullException(nameof(key));

      IQueryable<Template> query = _templates.ApplyTracking(readOnly).Include(x => x.Realm);

      return realm == null
        ? await query.SingleOrDefaultAsync(x => x.RealmSid == null && x.KeyNormalized == key, cancellationToken)
        : await query.SingleOrDefaultAsync(x => x.RealmSid == realm.Sid && x.KeyNormalized == key, cancellationToken);
    }

    public async Task<Template?> GetAsync(Guid id, bool readOnly, CancellationToken cancellationToken)
    {
      return await _templates.ApplyTracking(readOnly)
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PagedList<Template>> GetPagedAsync(Guid? realmId, string? search,
      TemplateSort? sort, bool desc,
      int? index, int? count,
      bool readOnly, CancellationToken cancellationToken)
    {
      IQueryable<Template> query = _templates.ApplyTracking(readOnly)
        .Include(x => x.Realm)
        .Where(x => realmId.HasValue ? (x.Realm != null && x.Realm.Id == realmId.Value) : x.Realm == null);

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
