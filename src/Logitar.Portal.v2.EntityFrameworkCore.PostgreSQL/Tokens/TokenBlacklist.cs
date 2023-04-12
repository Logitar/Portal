using Logitar.Portal.v2.Core.Tokens;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Tokens;

internal class TokenBlacklist : ITokenBlacklist
{
  private readonly PortalContext _context;

  public TokenBlacklist(PortalContext context)
  {
    _context = context;
  }

  public async Task BlacklistAsync(IEnumerable<Guid> ids, DateTime? expiresOn, CancellationToken cancellationToken = default)
  {
    _context.TokenBlacklist.AddRange(ids.Select(id => new BlacklistedTokenEntity(id, expiresOn)));

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task<IEnumerable<Guid>> GetBlacklistedAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
  {
    return await _context.TokenBlacklist.AsNoTracking()
      .Where(x => ids.Contains(x.Id) && (x.ExpiresOn == null || x.ExpiresOn > DateTime.UtcNow))
      .Select(x => x.Id)
      .ToArrayAsync(cancellationToken);
  }
}
