namespace Logitar.Portal.Contracts.Passwords;

public interface IOneTimePasswordClient
{
  Task<OneTimePassword> CreateAsync(CreateOneTimePasswordPayload payload, IRequestContext? context = null);
  Task<OneTimePassword?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<OneTimePassword?> ReadAsync(Guid id, IRequestContext? context = null);
  Task<OneTimePassword?> ValidateAsync(Guid id, ValidateOneTimePasswordPayload payload, IRequestContext? context = null);
}
