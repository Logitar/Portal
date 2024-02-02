namespace Logitar.Portal.Contracts.Users;

public interface IUserService
{
  Task<User> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken = default);
  Task<User?> SignOutAsync(Guid id, CancellationToken cancellationToken = default);
}
