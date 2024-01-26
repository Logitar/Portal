using Logitar.Identity.Domain.Sessions;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal class SignOutSessionCommandHandler : IRequestHandler<SignOutSessionCommand, Session?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;

  public SignOutSessionCommandHandler(IApplicationContext applicationContext, ISessionQuerier sessionQuerier, ISessionRepository sessionRepository)
  {
    _applicationContext = applicationContext;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
  }

  public async Task<Session?> Handle(SignOutSessionCommand command, CancellationToken cancellationToken)
  {
    SessionId id = new(command.Id);
    SessionAggregate? session = await _sessionRepository.LoadAsync(id, cancellationToken);
    if (session == null)
    {
      return null;
    }

    session.SignOut(_applicationContext.ActorId);

    await _sessionRepository.SaveAsync(session, cancellationToken);

    return await _sessionQuerier.ReadAsync(session, cancellationToken);
  }
}
