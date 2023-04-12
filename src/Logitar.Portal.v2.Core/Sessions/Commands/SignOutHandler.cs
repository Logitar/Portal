using Logitar.Portal.v2.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.v2.Core.Sessions.Commands;

internal class SignOutHandler : IRequestHandler<SignOut, Session>
{
  private readonly ICurrentActor _currentActor;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;

  public SignOutHandler(ICurrentActor currentActor,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository)
  {
    _currentActor = currentActor;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
  }

  public async Task<Session> Handle(SignOut request, CancellationToken cancellationToken)
  {
    SessionAggregate session = await _sessionRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<SessionAggregate>(request.Id);

    session.SignOut(_currentActor.Id);

    await _sessionRepository.SaveAsync(session, cancellationToken);

    return await _sessionQuerier.GetAsync(session, cancellationToken);
  }
}
