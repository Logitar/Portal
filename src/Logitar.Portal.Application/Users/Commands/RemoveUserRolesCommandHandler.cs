using Logitar.Portal.Application.Roles.Commands;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class RemoveUserRolesCommandHandler : INotificationHandler<RemoveRolesCommand>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserRepository _userRepository;

  public RemoveUserRolesCommandHandler(IApplicationContext applicationContext, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userRepository = userRepository;
  }

  public async Task Handle(RemoveRolesCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(command.Role, cancellationToken);
    foreach (UserAggregate user in users)
    {
      user.RemoveRole(command.Role);
      user.Update(_applicationContext.ActorId);
    }
    await _userRepository.SaveAsync(users, cancellationToken);
  }
}
