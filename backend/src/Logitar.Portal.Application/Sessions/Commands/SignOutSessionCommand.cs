using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

public record SignOutSessionCommand(Guid Id) : Activity, IRequest<SessionModel?>;

internal class SignOutSessionCommandHandler : IRequestHandler<SignOutSessionCommand, SessionModel?>
{
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public SignOutSessionCommandHandler(ISessionQuerier sessionQuerier, ISessionRepository sessionRepository, IUserRepository userRepository)
  {
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userRepository = userRepository;
  }

  public async Task<SessionModel?> Handle(SignOutSessionCommand command, CancellationToken cancellationToken)
  {
    SessionAggregate? session = await _sessionRepository.LoadAsync(command.Id, cancellationToken);
    if (session == null)
    {
      return null;
    }

    UserAggregate user = await _userRepository.LoadAsync(session, cancellationToken);
    if (user.TenantId != command.TenantId)
    {
      return null;
    }

    session.SignOut(command.ActorId);

    await _sessionRepository.SaveAsync(session, cancellationToken);

    return await _sessionQuerier.ReadAsync(command.Realm, session, cancellationToken);
  }
}
