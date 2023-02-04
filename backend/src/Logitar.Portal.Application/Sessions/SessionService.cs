using Logitar.Portal.Application.Sessions.Commands;
using Logitar.Portal.Application.Sessions.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Application.Sessions
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
      SessionSort? sort, bool isDescending, int? index, int? count, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetSessionsQuery(isActive, isPersistent, realm, userId,
        sort, isDescending, index, count), cancellationToken);
    }

    public async Task<IEnumerable<SessionModel>> SignOutAllAsync(string userId, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new SignOutUserSessionsCommand(userId), cancellationToken);
    }

    public async Task<SessionModel> SignOutAsync(string id, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new SignOutCommand(id), cancellationToken);
    }
  }
}
