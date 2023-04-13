using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Core.Realms;
using MediatR;

namespace Logitar.Portal.Core.Senders.Commands;

internal class DeleteSenderHandler : IRequestHandler<DeleteSender, Sender>
{
  private readonly ICurrentActor _currentActor;
  private readonly IRealmRepository _realmRepository;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public DeleteSenderHandler(ICurrentActor currentActor,
    IRealmRepository realmRepository,
    ISenderQuerier senderQuerier,
    ISenderRepository senderRepository)
  {
    _currentActor = currentActor;
    _realmRepository = realmRepository;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender> Handle(DeleteSender request, CancellationToken cancellationToken)
  {
    SenderAggregate sender = await _senderRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<SenderAggregate>(request.Id);
    RealmAggregate realm = await _realmRepository.LoadAsync(sender, cancellationToken);
    Sender output = await _senderQuerier.GetAsync(sender, cancellationToken);

    if (sender.IsDefault)
    {
      IEnumerable<SenderAggregate> senders = await _senderRepository.LoadAsync(realm, cancellationToken);
      if (senders.Any(s => !s.Equals(sender)))
      {
        throw new CannotDeleteDefaultSenderException(sender, _currentActor.Id);
      }
    }

    if (realm.PasswordRecoverySenderId == sender.Id)
    {
      realm.SetPasswordRecoverySender(_currentActor.Id, sender: null);
      await _realmRepository.SaveAsync(realm, cancellationToken);
    }

    sender.Delete(_currentActor.Id);

    await _senderRepository.SaveAsync(sender, cancellationToken);

    return output;
  }
}
