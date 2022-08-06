using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Sessions
{
  public interface ISessionService
  {
    Task<SessionModel> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ListModel<SessionModel>> GetAsync(bool? isActive = null, bool? isPersistent = null, string? realm = null, Guid? userId = null,
      SessionSort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
    Task<SessionModel> RenewAsync(Session session, string? ipAddress = null, string? additionalInformation = null, CancellationToken cancellationToken = default);
    Task<SessionModel> SignInAsync(User user, bool remember = false, string? ipAddress = null, string? additionalInformation = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<SessionModel>> SignOutAllAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<SessionModel> SignOutAsync(Guid id, CancellationToken cancellationToken = default);
  }
}
