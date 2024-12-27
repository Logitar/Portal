using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record RemoveUserIdentifierCommand(Guid Id, string Key) : Activity, IRequest<UserModel?>;

internal class RemoveUserIdentifierCommandHandler : IRequestHandler<RemoveUserIdentifierCommand, UserModel?>
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

  public async Task<UserModel?> Handle(RemoveUserIdentifierCommand command, CancellationToken cancellationToken)
  {
    UserId userId = new(command.TenantId, new EntityId(command.Id));
    User? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null || user.TenantId != command.TenantId)
    {
      return null;
    }

    Identifier key = new(command.Key); // TODO(fpion): will throw!
    ActorId actorId = command.ActorId;
    user.RemoveCustomIdentifier(key, actorId);
    await _userManager.SaveAsync(user, command.UserSettings, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(command.Realm, user, cancellationToken);
  }
}
