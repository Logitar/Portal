using Logitar.Portal.Core;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Sessions.Models;

namespace Logitar.Portal.Client
{
  public interface ISessionService
  {
    Task<SessionModel> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ListModel<SessionSummary>> GetAsync(bool? isActive = null, bool? isPersistent = null, string? realm = null, Guid? userId = null,
      SessionSort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
    Task<IEnumerable<SessionModel>> SignOutAllAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<SessionModel> SignOutAsync(Guid id, CancellationToken cancellationToken = default);
  }
}
