using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Contracts.Users;

public interface IUserService
{
  Task<User> ChangePasswordAsync(Guid id, ChangePasswordInput input, CancellationToken cancellationToken = default);
  Task<User> CreateAsync(CreateUserInput input, CancellationToken cancellationToken = default);
  Task<User> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User> DisableAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User> EnableAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User?> GetAsync(Guid? id = null, string? realm = null, string? username = null,
    string? externalKey = null, string? externalValue = null, CancellationToken cancellationToken = default);
  Task<PagedList<User>> GetAsync(bool? isConfirmed = null, bool? isDisabled = null, string? realm = null, string? search = null,
    UserSort? sort = null, bool isDescending = false, int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
  Task<SentMessages> RecoverPasswordAsync(RecoverPasswordInput input, CancellationToken cancellationToken = default);
  Task<User> SetExternalIdentifierAsync(Guid id, string key, string? value, CancellationToken cancellationToken = default);
  Task<User> UpdateAsync(Guid id, UpdateUserInput input, CancellationToken cancellationToken = default);
  Task<User> VerifyAddressAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User> VerifyEmailAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User> VerifyPhoneAsync(Guid id, CancellationToken cancellationToken = default);
}
