using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Shared;
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
    IEnumerable<OneTimePasswordAggregate> oneTimePasswords = await _oneTimePasswordRepository.LoadAsync(tenantId, cancellationToken);

    foreach (OneTimePasswordAggregate oneTimePassword in oneTimePasswords)
    {
      oneTimePassword.Delete(command.ActorId);
    }

    await _oneTimePasswordRepository.SaveAsync(oneTimePasswords, cancellationToken);
  }
}
