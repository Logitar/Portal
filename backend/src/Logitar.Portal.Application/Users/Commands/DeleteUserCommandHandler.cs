using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, User?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public DeleteUserCommandHandler(IApplicationContext applicationContext,
    IUserManager userManager, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
  {
    UserAggregate? user = await _userRepository.LoadAsync(command.Id, cancellationToken);
    if (user == null || user.TenantId != _applicationContext.TenantId)
    {
      return null;
    }
    User result = await _userQuerier.ReadAsync(user, cancellationToken);

    ActorId actorId = _applicationContext.ActorId;
    user.Delete(actorId);
    await _userManager.SaveAsync(user, actorId, cancellationToken);

    return result;
  }
}
