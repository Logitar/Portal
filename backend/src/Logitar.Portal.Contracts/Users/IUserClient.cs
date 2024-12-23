using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Users;

public interface IUserClient
{
  Task<UserModel> AuthenticateAsync(AuthenticateUserPayload payload, IRequestContext? context = null);
  Task<UserModel> CreateAsync(CreateUserPayload payload, IRequestContext? context = null);
  Task<UserModel?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<UserModel?> ReadAsync(Guid? id = null, string? uniqueName = null, CustomIdentifierModel? identifier = null, IRequestContext? context = null);
  Task<UserModel?> RemoveIdentifierAsync(Guid id, string key, IRequestContext? context = null);
  Task<UserModel?> ReplaceAsync(Guid id, ReplaceUserPayload payload, long? version = null, IRequestContext? context = null);
  Task<UserModel?> ResetPasswordAsync(Guid id, ResetUserPasswordPayload payload, IRequestContext? context = null);
  Task<UserModel?> SaveIdentifierAsync(Guid id, string key, SaveUserIdentifierPayload payload, IRequestContext? context = null);
  Task<SearchResults<UserModel>> SearchAsync(SearchUsersPayload payload, IRequestContext? context = null);
  Task<UserModel?> SignOutAsync(Guid id, IRequestContext? context = null);
  Task<UserModel?> UpdateAsync(Guid id, UpdateUserPayload payload, IRequestContext? context = null);
}
