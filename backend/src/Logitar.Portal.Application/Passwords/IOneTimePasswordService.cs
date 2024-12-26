using Logitar.Portal.Contracts.Passwords;

namespace Logitar.Portal.Application.Passwords;

public interface IOneTimePasswordService
{
  Task<OneTimePasswordModel> CreateAsync(CreateOneTimePasswordPayload payload, CancellationToken cancellationToken = default);
  Task<OneTimePasswordModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<OneTimePasswordModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<OneTimePasswordModel?> ValidateAsync(Guid id, ValidateOneTimePasswordPayload payload, CancellationToken cancellationToken = default);
}
