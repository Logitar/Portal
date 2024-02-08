using Logitar.Identity.Domain.Passwords;
using Logitar.Portal.Contracts.Passwords;

namespace Logitar.Portal.Application.OneTimePasswords;

public interface IOneTimePasswordQuerier
{
  Task<OneTimePassword> ReadAsync(OneTimePasswordAggregate oneTimePassword, CancellationToken cancellationToken = default);
  Task<OneTimePassword?> ReadAsync(OneTimePasswordId id, CancellationToken cancellationToken = default);
  Task<OneTimePassword?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
