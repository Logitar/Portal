using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Application.Sessions
{
  public interface ISessionService
  {
    Task<SessionModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<SessionModel>> GetAsync(bool? isActive = null, bool? isPersistent = null, string? realm = null, string? userId = null,
      SessionSort? sort = null, bool isDescending = false, int? index = null, int? count = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<SessionModel>> SignOutAllAsync(string userId, CancellationToken cancellationToken = default);
    Task<SessionModel> SignOutAsync(string id, CancellationToken cancellationToken = default);
  }
}
