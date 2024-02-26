using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal class SignOutSessionCommandHandler : IRequestHandler<SignOutSessionCommand, Session?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public SignOutSessionCommandHandler(IApplicationContext applicationContext, ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userRepository = userRepository;
  }

  public async Task<Session?> Handle(SignOutSessionCommand command, CancellationToken cancellationToken)
  {
    SessionAggregate? session = await _sessionRepository.LoadAsync(command.Id, cancellationToken);
    if (session == null)
    {
      return null;
    }

    UserAggregate user = await _userRepository.LoadAsync(session, cancellationToken);
    if (user.TenantId != _applicationContext.TenantId)
    {
      return null;
    }

    session.SignOut(_applicationContext.ActorId);

    await _sessionRepository.SaveAsync(session, cancellationToken);

    return await _sessionQuerier.ReadAsync(session, cancellationToken);
  }
}
