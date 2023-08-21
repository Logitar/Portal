namespace Logitar.Portal.Contracts.Users;

public interface IUserService
{
  Task<User> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken = default);
  Task<User?> DeleteAsync(string id, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(string? id = null, string? realm = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken = default);
  Task<User?> UpdateAsync(string id, UpdateUserPayload payload, CancellationToken cancellationToken = default);
}
