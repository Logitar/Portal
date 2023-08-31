using Logitar.Portal.Application.Sessions.Commands;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, User?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPublisher _publisher;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public DeleteUserCommandHandler(IApplicationContext applicationContext, IPublisher publisher,
    IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _publisher = publisher;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
  {
    UserAggregate? user = await _userRepository.LoadAsync(command.Id, cancellationToken);
    if (user == null)
    {
      return null;
    }
    User result = await _userQuerier.ReadAsync(user, cancellationToken);

    await _publisher.Publish(new DeleteSessionsCommand(user), cancellationToken);

    user.Delete(_applicationContext.ActorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return result;
  }
}
