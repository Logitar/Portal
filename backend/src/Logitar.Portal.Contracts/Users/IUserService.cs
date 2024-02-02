namespace Logitar.Portal.Contracts.Users;

public interface IUserService
{
  Task<User> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken = default);
  Task<User?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(Guid? id = null, string? uniqueName = null, CustomIdentifier? identifier = null, CancellationToken cancellationToken = default);
  Task<User?> SignOutAsync(Guid id, CancellationToken cancellationToken = default);
}
