using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Sessions;
using Logitar.Portal.v2.Core.Sessions.Commands;
using Logitar.Portal.v2.Core.Sessions.Queries;

namespace Logitar.Portal.v2.Core.Sessions;

internal class SessionService : ISessionService
{
  private readonly IRequestPipeline _pipeline;

  public SessionService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<Session?> GetAsync(Guid? id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new GetSession(id), cancellationToken);
  }

  public async Task<PagedList<Session>> GetAsync(bool? isActive, bool? isPersistent, string? realm, Guid? userId,
    SessionSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new GetSessions(isActive, isPersistent, realm, userId,
      sort, isDescending, skip, limit), cancellationToken);
  }

  public async Task<Session> RefreshAsync(RefreshInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new Refresh(input), cancellationToken);
  }

  public async Task<Session> SignInAsync(SignInInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SignIn(input), cancellationToken);
  }

  public async Task<Session> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SignOut(id), cancellationToken);
  }

  public async Task<IEnumerable<Session>> SignOutUserAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SignOutUser(id), cancellationToken);
  }
}
