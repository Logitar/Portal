using Logitar.Identity.Domain.Passwords;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Application.OneTimePasswords;

public interface IOneTimePasswordQuerier
{
  Task<OneTimePassword> ReadAsync(Realm? realm, OneTimePasswordAggregate oneTimePassword, CancellationToken cancellationToken = default);
  Task<OneTimePassword?> ReadAsync(Realm? realm, OneTimePasswordId id, CancellationToken cancellationToken = default);
  Task<OneTimePassword?> ReadAsync(Realm? realm, Guid id, CancellationToken cancellationToken = default);
}
