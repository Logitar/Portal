using Portal.Core.Users;

namespace Portal.Core.Sessions
{
  public interface ISessionQuerier
  {
    Task<Session?> GetAsync(Guid id, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<Session>> GetActiveAsync(User user, bool readOnly = false, CancellationToken cancellationToken = default);
  }
}
