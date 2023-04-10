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

  public async Task<Session> SignInAsync(SignInInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SignIn(input), cancellationToken);
  }
}
