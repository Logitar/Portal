using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Users
{
  public interface IUserRepository : IRepository
  {
    Task<User?> LoadByUsernameAsync(string username, AggregateId? realmId = null, CancellationToken cancellationToken = default);
  }
}
