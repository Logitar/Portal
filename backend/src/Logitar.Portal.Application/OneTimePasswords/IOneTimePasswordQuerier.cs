using Logitar.Identity.Core.Passwords;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Application.OneTimePasswords;

public interface IOneTimePasswordQuerier
{
  Task<OneTimePasswordModel> ReadAsync(RealmModel? realm, OneTimePassword oneTimePassword, CancellationToken cancellationToken = default);
  Task<OneTimePasswordModel?> ReadAsync(RealmModel? realm, OneTimePasswordId id, CancellationToken cancellationToken = default);
  Task<OneTimePasswordModel?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken = default);
}
