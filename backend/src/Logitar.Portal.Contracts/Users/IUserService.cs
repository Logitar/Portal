namespace Logitar.Portal.Contracts.Users;

public interface IUserService
{
  Task<User> AuthenticateAsync(AuthenticateUserPayload payload, CancellationToken cancellationToken = default);
  Task<User> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken = default);
  Task<User?> ReplaceAsync(string id, ReplaceUserPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<User?> SignOutAsync(string id, CancellationToken cancellationToken = default);
  Task<User?> UpdateAsync(string id, UpdateUserPayload payload, CancellationToken cancellationToken = default);
}
