namespace Logitar.Portal.Contracts.Users;

public interface IUserService
{
  Task<User> AuthenticateAsync(AuthenticateUserPayload payload, CancellationToken cancellationToken = default);
  Task<User> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken = default);
  Task<User?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(Guid? id = null, string? realm = null, string? uniqueName = null,
    string? identifierKey = null, string? identifierValue = null, CancellationToken cancellationToken = default);
  Task<User?> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken = default);
  Task<User?> ReplaceAsync(Guid id, ReplaceUserPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<User?> SaveIdentifierAsync(Guid id, SaveIdentifierPayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken = default);
  Task<User?> SignOutAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User?> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken = default);
}
