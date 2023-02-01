using Logitar.Portal.Contracts.Users.Models;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Users
{
  public interface IUserQuerier
  {
    Task<UserModel?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
    Task<UserModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserModel>> GetByEmailAsync(string email, Realm realm, CancellationToken cancellationToken = default);
  }
}
