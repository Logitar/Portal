using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

public record SignOutUserCommand(Guid Id) : Activity, IRequest<UserModel?>;

internal class SignOutUserCommandHandler : IRequestHandler<SignOutUserCommand, UserModel?>
{
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public SignOutUserCommandHandler(ISessionRepository sessionRepository, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _sessionRepository = sessionRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<UserModel?> Handle(SignOutUserCommand command, CancellationToken cancellationToken)
  {
    UserId userId = new(command.TenantId, new EntityId(command.Id));
    User? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null || user.TenantId != command.TenantId)
    {
      return null;
    }

    ActorId actorId = command.ActorId;
    IReadOnlyCollection<Session> sessions = await _sessionRepository.LoadActiveAsync(user, cancellationToken);
    foreach (Session session in sessions)
    {
      session.SignOut(actorId);
    }

    await _sessionRepository.SaveAsync(sessions, cancellationToken);

    return await _userQuerier.ReadAsync(command.Realm, user, cancellationToken);
  }
}
