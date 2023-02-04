using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Tokens
{
  internal class JwtBlacklist : IJwtBlacklist
  {
    private readonly PortalContext _context;

    public JwtBlacklist(PortalContext dbContext)
    {
      _context = dbContext;
    }

    public async Task BlacklistAsync(IEnumerable<Guid> ids, DateTime? expiresOn, CancellationToken cancellationToken)
    {
      _context.JwtBlacklist.AddRange(ids.Select(id => new BlacklistedJwtEntity(id, expiresOn)));

      await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Guid>> GetBlacklistedAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
      return await _context.JwtBlacklist.AsNoTracking()
        .Where(x => ids.Contains(x.Id) && (x.ExpiresOn == null || x.ExpiresOn > DateTime.UtcNow))
        .Select(x => x.Id)
        .ToArrayAsync(cancellationToken);
    }
  }
}
