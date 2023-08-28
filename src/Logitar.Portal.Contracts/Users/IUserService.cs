namespace Logitar.Portal.Contracts.Users;

public interface IUserService
{
  Task<User> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken = default);
  Task<User?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(Guid? id = null, string? realm = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<User?> ReplaceAsync(Guid id, ReplaceUserPayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken = default);
  Task<User?> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken = default);
}
