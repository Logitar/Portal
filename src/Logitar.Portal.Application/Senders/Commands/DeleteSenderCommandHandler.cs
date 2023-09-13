using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal class DeleteSenderCommandHandler : IRequestHandler<DeleteSenderCommand, Sender?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public DeleteSenderCommandHandler(IApplicationContext applicationContext, IRealmRepository realmRepository,
    ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender?> Handle(DeleteSenderCommand command, CancellationToken cancellationToken)
  {
    SenderAggregate? sender = await _senderRepository.LoadAsync(command.Id, cancellationToken);
    if (sender == null)
    {
      return null;
    }
    RealmAggregate? realm = await _realmRepository.LoadAsync(sender, cancellationToken);
    Sender result = await _senderQuerier.ReadAsync(sender, cancellationToken);

    if (sender.IsDefault)
    {
      IEnumerable<SenderAggregate> senders = await _senderRepository.LoadAsync(realm, cancellationToken);
      if (senders.Count() > 1)
      {
        throw new CannotDeleteDefaultSenderException(sender);
      }
    }

    sender.Delete(_applicationContext.ActorId);

    if (realm?.PasswordRecoverySenderId == sender.Id)
    {
      realm.RemovePasswordRecoverySender();
      realm.Update(_applicationContext.ActorId);

      await _realmRepository.SaveAsync(realm, cancellationToken);
    }

    await _senderRepository.SaveAsync(sender, cancellationToken);

    return result;
  }
}
