using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Tokens
{
  internal class JwtBlacklist : IJwtBlacklist
  {
    private readonly PortalDbContext _dbContext;

    public JwtBlacklist(PortalDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task BlacklistAsync(IEnumerable<Guid> ids, DateTime? expiresAt, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(ids);

      _dbContext.JwtBlacklist.AddRange(ids.Select(id => new BlacklistedJwt(id, expiresAt)));

      await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Guid>> GetBlacklistedAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(ids);

      return await _dbContext.JwtBlacklist.AsNoTracking()
        .Where(x => ids.Contains(x.Id) && (x.ExpiresAt == null || x.ExpiresAt > DateTime.UtcNow))
        .Select(x => x.Id)
        .ToArrayAsync(cancellationToken);
    }
  }
}
