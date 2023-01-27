using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Sessions.Models;

namespace Logitar.Portal.Core.Sessions
{
  public interface ISessionQuerier
  {
    Task<SessionModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<SessionModel>> GetPagedAsync(bool? isActive = null, bool? isPersistent = null, string? realm = null, string? userId = null,
      SessionSort? sort = null, bool isDescending = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
  }
}
