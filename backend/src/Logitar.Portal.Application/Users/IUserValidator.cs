using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Users
{
  public interface IUserValidator
  {
    void ValidateAndThrow(User user, UsernameSettings? usernameSettings = null);
    Task ValidateAndThrowAsync(User user, CancellationToken cancellationToken = default);
  }
}
