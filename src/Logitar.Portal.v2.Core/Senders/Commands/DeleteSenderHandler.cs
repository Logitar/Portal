﻿using Logitar.Portal.v2.Contracts.Senders;
using Logitar.Portal.v2.Core.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Senders.Commands;

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
    Sender output = await _senderQuerier.GetAsync(sender, cancellationToken);

    if (sender.IsDefault)
    {
      RealmAggregate realm = await _realmRepository.LoadAsync(sender, cancellationToken);
      IEnumerable<SenderAggregate> senders = await _senderRepository.LoadAsync(realm, cancellationToken);
      if (senders.Any(s => !s.Equals(sender)))
      {
        throw new CannotDeleteDefaultSenderException(sender, _currentActor.Id);
      }
    }

    sender.Delete(_currentActor.Id);

    await _senderRepository.SaveAsync(sender, cancellationToken);

    return output;
  }
}
