namespace Logitar.Portal.Domain.Users;

public interface IUserRepository
{
  Task SaveAsync(UserAggregate user, CancellationToken cancellationToken = default);
}
