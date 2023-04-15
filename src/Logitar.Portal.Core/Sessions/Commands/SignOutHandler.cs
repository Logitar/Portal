using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands;

internal class SignOutHandler : IRequestHandler<SignOut, Session>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;

  public SignOutHandler(IApplicationContext applicationContext,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository)
  {
    _applicationContext = applicationContext;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
  }

  public async Task<Session> Handle(SignOut request, CancellationToken cancellationToken)
  {
    SessionAggregate session = await _sessionRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<SessionAggregate>(request.Id);

    session.SignOut(_applicationContext.ActorId);

    await _sessionRepository.SaveAsync(session, cancellationToken);

    return await _sessionQuerier.GetAsync(session, cancellationToken);
  }
}
