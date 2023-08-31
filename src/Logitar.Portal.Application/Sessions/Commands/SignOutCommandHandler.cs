using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal class SignOutCommandHandler : IRequestHandler<SignOutCommand, Session?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;

  public SignOutCommandHandler(IApplicationContext applicationContext, ISessionQuerier sessionQuerier, ISessionRepository sessionRepository)
  {
    _applicationContext = applicationContext;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
  }

  public async Task<Session?> Handle(SignOutCommand command, CancellationToken cancellationToken)
  {
    SessionAggregate? session = await _sessionRepository.LoadAsync(command.Id, cancellationToken);
    if (session == null)
    {
      return null;
    }

    session.SignOut(_applicationContext.ActorId);

    await _sessionRepository.SaveAsync(session, cancellationToken);

    return await _sessionQuerier.ReadAsync(session, cancellationToken);
  }
}
