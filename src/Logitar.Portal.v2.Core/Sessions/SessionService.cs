using Logitar.Portal.v2.Contracts.Sessions;
using Logitar.Portal.v2.Core.Sessions.Commands;

namespace Logitar.Portal.v2.Core.Sessions;

internal class SessionService : ISessionService
{
  private readonly IRequestPipeline _pipeline;

  public SessionService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
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
