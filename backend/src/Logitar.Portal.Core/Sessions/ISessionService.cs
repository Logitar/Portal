using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Sessions.Models;

namespace Logitar.Portal.Core.Sessions
{
  internal interface ISessionService
  {
    Task<SessionModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<SessionModel>> GetAsync(bool? isActive = null, bool? isPersistent = null, string? realm = null, string? userId = null,
      SessionSort? sort = null, bool isDescending = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
    Task<IEnumerable<SessionModel>> SignOutAllAsync(string userId, CancellationToken cancellationToken = default);
    Task<SessionModel> SignOutAsync(string id, CancellationToken cancellationToken = default);
  }
}
