using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Users;

public interface IUserClient
{
  Task<User> AuthenticateAsync(AuthenticateUserPayload payload, IRequestContext? context = null);
  Task<User> CreateAsync(CreateUserPayload payload, IRequestContext? context = null);
  Task<User?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<User?> ReadAsync(Guid? id = null, string? uniqueName = null, CustomIdentifier? identifier = null, IRequestContext? context = null);
  Task<User?> RemoveIdentifierAsync(Guid id, string key, IRequestContext? context = null);
  Task<User?> ReplaceAsync(Guid id, ReplaceUserPayload payload, long? version = null, IRequestContext? context = null);
  Task<User?> ResetPasswordAsync(Guid id, ResetUserPasswordPayload payload, IRequestContext? context = null);
  Task<User?> SaveIdentifierAsync(Guid id, string key, SaveUserIdentifierPayload payload, IRequestContext? context = null);
  Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, IRequestContext? context = null);
  Task<User?> SignOutAsync(Guid id, IRequestContext? context = null);
  Task<User?> UpdateAsync(Guid id, UpdateUserPayload payload, IRequestContext? context = null);
}
