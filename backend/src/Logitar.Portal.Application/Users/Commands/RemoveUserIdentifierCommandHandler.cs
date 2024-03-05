using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class RemoveUserIdentifierCommandHandler : IRequestHandler<RemoveUserIdentifierCommand, User?>
{
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public RemoveUserIdentifierCommandHandler(IUserManager userManager, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(RemoveUserIdentifierCommand command, CancellationToken cancellationToken)
  {
    UserAggregate? user = await _userRepository.LoadAsync(command.Id, cancellationToken);
    if (user == null || user.TenantId != command.TenantId)
    {
      return null;
    }

    ActorId actorId = command.ActorId;
    user.RemoveCustomIdentifier(command.Key, actorId);
    await _userManager.SaveAsync(user, command.UserSettings, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(command.Realm, user, cancellationToken);
  }
}
