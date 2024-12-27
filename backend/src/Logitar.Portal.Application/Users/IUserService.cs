using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users;

public interface IUserService
{
  Task<UserModel> AuthenticateAsync(AuthenticateUserPayload payload, CancellationToken cancellationToken = default);
  Task<UserModel> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken = default);
  Task<UserModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<UserModel?> ReadAsync(Guid? id = null, string? uniqueName = null, CustomIdentifierModel? identifier = null, CancellationToken cancellationToken = default);
  Task<UserModel?> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken = default);
  Task<UserModel?> ReplaceAsync(Guid id, ReplaceUserPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<UserModel?> ResetPasswordAsync(Guid id, ResetUserPasswordPayload payload, CancellationToken cancellationToken = default);
  Task<UserModel?> SaveIdentifierAsync(Guid id, string key, SaveUserIdentifierPayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<UserModel>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken = default);
  Task<UserModel?> SignOutAsync(Guid id, CancellationToken cancellationToken = default);
  Task<UserModel?> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken = default);
}
