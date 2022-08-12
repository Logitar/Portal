using Microsoft.EntityFrameworkCore;
using Logitar.Portal.Core;
using Logitar.Portal.Core.ApiKeys;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class ApiKeyQuerier : IApiKeyQuerier
  {
    private readonly DbSet<ApiKey> _apiKeys;

    public ApiKeyQuerier(PortalDbContext dbContext)
    {
      _apiKeys = dbContext.ApiKeys;
    }

    public async Task<ApiKey?> GetAsync(Guid id, bool readOnly, CancellationToken cancellationToken)
    {
      return await _apiKeys.ApplyTracking(readOnly)
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PagedList<ApiKey>> GetPagedAsync(bool? isExpired, string? search,
      ApiKeySort? sort, bool desc,
      int? index, int? count,
      bool readOnly, CancellationToken cancellationToken)
    {
      IQueryable<ApiKey> query = _apiKeys.ApplyTracking(readOnly);

      if (isExpired.HasValue)
      {
        query = query.Where(x => x.IsExpired == isExpired.Value);
      }
      if (search != null)
      {
        foreach (string term in search.Split())
        {
          if (!string.IsNullOrEmpty(term))
          {
            string pattern = $"%{term}%";

            query = query.Where(x => EF.Functions.ILike(x.Name, pattern));
          }
        }
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        query = sort.Value switch
        {
          ApiKeySort.ExpiresAt => desc ? query.OrderByDescending(x => x.ExpiresAt) : query.OrderBy(x => x.ExpiresAt),
          ApiKeySort.Name => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
          ApiKeySort.UpdatedAt => desc ? query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt) : query.OrderBy(x => x.UpdatedAt ?? x.CreatedAt),
          _ => throw new ArgumentException($"The API key sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      ApiKey[] apiKeys = await query.ToArrayAsync(cancellationToken);

      return new PagedList<ApiKey>(apiKeys, total);
    }
  }
}
