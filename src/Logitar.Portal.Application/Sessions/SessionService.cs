using Logitar.Portal.Application.Sessions.Commands;
using Logitar.Portal.Application.Sessions.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Application.Sessions;

internal class SessionService : ISessionService
{
  private readonly IRequestPipeline _pipeline;

  public SessionService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<Session> CreateAsync(CreateSessionPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateSessionCommand(payload), cancellationToken);
  }

  public async Task<Session?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReadSessionQuery(id), cancellationToken);
  }

  public async Task<Session> RenewAsync(RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new RenewSessionCommand(payload), cancellationToken);
  }

  public async Task<SearchResults<Session>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SearchSessionsQuery(payload), cancellationToken);
  }

  public async Task<Session> SignInAsync(SignInPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SignInCommand(payload), cancellationToken);
  }

  public async Task<Session?> SignOutAsync(string id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SignOutSessionCommand(id), cancellationToken);
  }
}
