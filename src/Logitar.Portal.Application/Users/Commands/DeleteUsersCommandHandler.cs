using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class DeleteUsersCommandHandler : INotificationHandler<DeleteUsersCommand>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserRepository _userRepository;

  public DeleteUsersCommandHandler(IApplicationContext applicationContext, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userRepository = userRepository;
  }

  public async Task Handle(DeleteUsersCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(command.Realm, cancellationToken);
    foreach (UserAggregate user in users)
    {
      user.Delete(_applicationContext.ActorId);
    }

    await _userRepository.SaveAsync(users, cancellationToken);
  }
}
