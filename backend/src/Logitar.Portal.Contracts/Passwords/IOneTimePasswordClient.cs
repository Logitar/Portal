namespace Logitar.Portal.Contracts.Passwords;

public interface IOneTimePasswordClient
{
  Task<OneTimePasswordModel> CreateAsync(CreateOneTimePasswordPayload payload, IRequestContext? context = null);
  Task<OneTimePasswordModel?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<OneTimePasswordModel?> ReadAsync(Guid id, IRequestContext? context = null);
  Task<OneTimePasswordModel?> ValidateAsync(Guid id, ValidateOneTimePasswordPayload payload, IRequestContext? context = null);
}
