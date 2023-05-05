using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Core.Sessions.Commands;
using Logitar.Portal.Core.Sessions.Queries;

namespace Logitar.Portal.Core.Sessions;

internal class SessionService : ISessionService
{
  private readonly IRequestPipeline _pipeline;

  public SessionService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<Session> CreateAsync(CreateSessionInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateSession(input), cancellationToken);
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

  public async Task<Session> SignInAsync(SignInInput input, string? realm, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SignIn(input, realm), cancellationToken);
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
