using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Core.Users;

public interface IUserQuerier
{
  Task<User> GetAsync(UserAggregate user, CancellationToken cancellationToken = default);
  Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default);
  Task<User?> GetAsync(string realm, string username, CancellationToken cancellationToken = default);
  Task<User?> GetAsync(string realm, string externalKey, string externalValue, CancellationToken cancellationToken = default);
  Task<PagedList<User>> GetAsync(bool? isConfirmed = null, bool? isDisabled = null, string? realm = null, string? search = null,
    UserSort? sort = null, bool isDescending = false, int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
}
