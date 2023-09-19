namespace Logitar.Portal.Application.Tokens;

public interface ITokenBlacklist
{
  Task BlacklistAsync(IEnumerable<Guid> ids, DateTime? expiresOn = null, CancellationToken cancellationToken = default);
  Task<IEnumerable<Guid>> FindBlacklistedAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
