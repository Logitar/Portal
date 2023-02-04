using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain;

namespace Logitar.Portal.Application.Sessions
{
  public interface ISessionQuerier
  {
    Task<SessionModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<SessionModel?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SessionModel>> GetAsync(IEnumerable<AggregateId> ids, CancellationToken cancellationToken = default);
    Task<ListModel<SessionModel>> GetPagedAsync(bool? isActive = null, bool? isPersistent = null, string? realm = null, string? userId = null,
      SessionSort? sort = null, bool isDescending = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
  }
}
