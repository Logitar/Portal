namespace Logitar.Portal.Contracts.Users;

public interface IUserService
{
  Task<User> AuthenticateAsync(AuthenticateUserPayload payload, CancellationToken cancellationToken = default);
  Task<User> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken = default);
  Task<User?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(Guid? id = null, string? uniqueName = null, CustomIdentifier? identifier = null, CancellationToken cancellationToken = default);
  Task<User?> ResetPasswordAsync(Guid id, ResetUserPasswordPayload payload, CancellationToken cancellationToken = default);
  Task<User?> SignOutAsync(Guid id, CancellationToken cancellationToken = default);
}
