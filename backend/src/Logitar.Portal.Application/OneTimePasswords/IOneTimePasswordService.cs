using Logitar.Portal.Contracts.Passwords;

namespace Logitar.Portal.Application.OneTimePasswords;

public interface IOneTimePasswordService
{
  Task<OneTimePassword> CreateAsync(CreateOneTimePasswordPayload payload, CancellationToken cancellationToken = default);
  Task<OneTimePassword?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<OneTimePassword?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<OneTimePassword?> ValidateAsync(Guid id, ValidateOneTimePasswordPayload payload, CancellationToken cancellationToken = default);
}
