namespace Logitar.Portal.Contracts.Users;

public interface IUserService
{
  Task<User?> SignOutAsync(Guid id, CancellationToken cancellationToken = default);
}
