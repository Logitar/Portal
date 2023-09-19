using Logitar.Portal.Application.Tokens;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Tokens;

internal class TokenBlacklist : ITokenBlacklist
{
  private readonly PortalContext _context;

  public TokenBlacklist(PortalContext context)
  {
    _context = context;
  }

  public async Task BlacklistAsync(IEnumerable<Guid> ids, DateTime? expiresOn, CancellationToken cancellationToken)
  {
    _context.TokenBlacklist.AddRange(ids.Select(id => new BlacklistedTokenEntity(id, expiresOn)));

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task<IEnumerable<Guid>> FindBlacklistedAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
  {
    return await _context.TokenBlacklist.AsNoTracking()
      .Where(x => ids.Contains(x.Id))
      .Select(x => x.Id)
      .ToArrayAsync(cancellationToken);
  }
}
