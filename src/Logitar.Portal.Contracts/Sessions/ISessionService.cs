namespace Logitar.Portal.Contracts.Sessions;

public interface ISessionService
{
  Task<Session?> GetAsync(Guid? id = null, CancellationToken cancellationToken = default);
  Task<PagedList<Session>> GetAsync(bool? isActive = null, bool? isPersistent = null, string? realm = null, Guid? userId = null,
    SessionSort? sort = null, bool isDescending = false, int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
  Task<Session> RefreshAsync(RefreshInput input, CancellationToken cancellationToken = default);
  Task<Session> SignInAsync(SignInInput input, CancellationToken cancellationToken = default);
  Task<Session> SignOutAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IEnumerable<Session>> SignOutUserAsync(Guid id, CancellationToken cancellationToken = default);
}
