using Logitar.EventSourcing;
using Logitar.Identity.Domain.Sessions;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal class SignOutSessionCommandHandler : IRequestHandler<SignOutSessionCommand, Session?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISessionManager _sessionManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;

  public SignOutSessionCommandHandler(IApplicationContext applicationContext,
    ISessionManager sessionManager, ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository)
  {
    _applicationContext = applicationContext;
    _sessionManager = sessionManager;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
  }

  public async Task<Session?> Handle(SignOutSessionCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = command.Id.GetAggregateId(nameof(command.Id));
    SessionAggregate? session = await _sessionRepository.LoadAsync(id, cancellationToken);
    if (session == null)
    {
      return null;
    }

    session.SignOut(_applicationContext.ActorId);

    await _sessionManager.SaveAsync(session, cancellationToken);

    return await _sessionQuerier.ReadAsync(session, cancellationToken);
  }
}
