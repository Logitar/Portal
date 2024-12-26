using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record DeleteUserCommand(Guid Id) : Activity, IRequest<UserModel?>;

internal class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, UserModel?>
{
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public DeleteUserCommandHandler(IUserManager userManager, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<UserModel?> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
  {
    UserAggregate? user = await _userRepository.LoadAsync(command.Id, cancellationToken);
    if (user == null || user.TenantId != command.TenantId)
    {
      return null;
    }
    UserModel result = await _userQuerier.ReadAsync(command.Realm, user, cancellationToken);

    ActorId actorId = command.ActorId;
    user.Delete(actorId);
    await _userManager.SaveAsync(user, command.UserSettings, actorId, cancellationToken);

    return result;
  }
}
