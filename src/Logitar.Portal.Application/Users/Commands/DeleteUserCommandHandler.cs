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

  public DeleteUserCommandHandler(IApplicationContext applicationContext, IUserManager userManager,
    IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = command.Id.GetAggregateId(nameof(command.Id));
    UserAggregate? user = await _userRepository.LoadAsync(id, cancellationToken);
    if (user == null)
    {
      return null;
    }
    User result = await _userQuerier.ReadAsync(user, cancellationToken);

    user.Delete(_applicationContext.ActorId);

    await _userManager.DeleteAsync(user, cancellationToken);

    return result;
  }
}
