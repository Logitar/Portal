using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Sessions.Commands;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Sessions.Queries;

namespace Logitar.Portal.Core.Sessions
{
  internal class SessionService : ISessionService
  {
    private readonly IRequestPipeline _requestPipeline;

    public SessionService(IRequestPipeline requestPipeline)
    {
      _requestPipeline = requestPipeline;
    }

    public async Task<SessionModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetSessionQuery(id), cancellationToken);
    }

    public async Task<ListModel<SessionModel>> GetAsync(bool? isActive, bool? isPersistent, string? realm, string? userId,
      SessionSort? sort, bool isDescending,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetSessionsQuery
      {
        IsActive = isActive,
        IsPersistent = isPersistent,
        Realm = realm,
        UserId = userId,
        Sort = sort,
        IsDescending = isDescending,
        Index = index,
        Count = count
      }, cancellationToken);
    }

    public async Task<IEnumerable<SessionModel>> SignOutAllAsync(string userId, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new SignOutSessionsCommand(userId), cancellationToken);
    }

    public async Task<SessionModel> SignOutAsync(string id, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new SignOutSessionCommand(id), cancellationToken);
    }
  }
}
