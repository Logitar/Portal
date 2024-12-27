using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.DeleteCommands;

internal record DeleteRealmUsersCommand(Realm Realm, ActorId ActorId) : INotification;

internal class DeleteRealmUsersCommandHandler : INotificationHandler<DeleteRealmUsersCommand>
{
  private readonly IUserRepository _userRepository;

  public DeleteRealmUsersCommandHandler(IUserRepository userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task Handle(DeleteRealmUsersCommand command, CancellationToken cancellationToken)
  {
    TenantId tenantId = new(command.Realm.Id.Value);
    IEnumerable<User> users = await _userRepository.LoadAsync(tenantId, cancellationToken);

    foreach (User user in users)
    {
      user.Delete(command.ActorId);
    }

    await _userRepository.SaveAsync(users, cancellationToken);
  }
}
