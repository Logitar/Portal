using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.DeleteCommands;

internal record DeleteRealmOneTimePasswordsCommand(Realm Realm, ActorId ActorId) : INotification;

internal class DeleteRealmOneTimePasswordsCommandHandler : INotificationHandler<DeleteRealmOneTimePasswordsCommand>
{
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;

  public DeleteRealmOneTimePasswordsCommandHandler(IOneTimePasswordRepository oneTimePasswordRepository)
  {
    _oneTimePasswordRepository = oneTimePasswordRepository;
  }

  public async Task Handle(DeleteRealmOneTimePasswordsCommand command, CancellationToken cancellationToken)
  {
    TenantId tenantId = new(command.Realm.Id.Value);
    IEnumerable<OneTimePassword> oneTimePasswords = await _oneTimePasswordRepository.LoadAsync(tenantId, cancellationToken);

    foreach (OneTimePassword oneTimePassword in oneTimePasswords)
    {
      oneTimePassword.Delete(command.ActorId);
    }

    await _oneTimePasswordRepository.SaveAsync(oneTimePasswords, cancellationToken);
  }
}
