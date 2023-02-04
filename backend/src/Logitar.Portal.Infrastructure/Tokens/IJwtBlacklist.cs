namespace Logitar.Portal.Infrastructure.Tokens
{
  internal interface IJwtBlacklist
  {
    Task BlacklistAsync(IEnumerable<Guid> ids, DateTime? expiresOn = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Guid>> GetBlacklistedAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
  }
}
