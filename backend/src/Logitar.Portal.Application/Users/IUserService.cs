using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users;

public interface IUserService
{
  Task<User> AuthenticateAsync(AuthenticateUserPayload payload, CancellationToken cancellationToken = default);
  Task<User> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken = default);
  Task<User?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(Guid? id = null, string? uniqueName = null, CustomIdentifierModel? identifier = null, CancellationToken cancellationToken = default);
  Task<User?> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken = default);
  Task<User?> ReplaceAsync(Guid id, ReplaceUserPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<User?> ResetPasswordAsync(Guid id, ResetUserPasswordPayload payload, CancellationToken cancellationToken = default);
  Task<User?> SaveIdentifierAsync(Guid id, string key, SaveUserIdentifierPayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken = default);
  Task<User?> SignOutAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User?> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken = default);
}
