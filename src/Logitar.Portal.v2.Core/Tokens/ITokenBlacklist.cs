namespace Logitar.Portal.v2.Core.Tokens;

public interface ITokenBlacklist
{
  Task<IEnumerable<Guid>> GetBlacklistedAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
  Task BlacklistAsync(IEnumerable<Guid> ids, DateTime? expiresOn = null, CancellationToken cancellationToken = default);
}
