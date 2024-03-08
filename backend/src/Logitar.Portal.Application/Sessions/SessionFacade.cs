using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Sessions.Commands;
using Logitar.Portal.Application.Sessions.Queries;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Application.Sessions;

internal class SessionFacade : ISessionService
{
  private readonly IActivityPipeline _activityPipeline;

  public SessionFacade(IActivityPipeline activityPipeline)
  {
    _activityPipeline = activityPipeline;
  }

  public async Task<Session> CreateAsync(CreateSessionPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new CreateSessionCommand(payload), cancellationToken);
  }

  public async Task<Session?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReadSessionQuery(id), cancellationToken);
  }

  public async Task<Session> RenewAsync(RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new RenewSessionCommand(payload), cancellationToken);
  }

  public async Task<SearchResults<Session>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SearchSessionsQuery(payload), cancellationToken);
  }

  public async Task<Session> SignInAsync(SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SignInSessionCommand(payload), cancellationToken);
  }

  public async Task<Session?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SignOutSessionCommand(id), cancellationToken);
  }
}
