using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Core.Realms;
using MediatR;

namespace Logitar.Portal.Core.Senders.Commands;

internal class SetDefaultSenderHandler : IRequestHandler<SetDefaultSender, Sender>
{
  private readonly ICurrentActor _currentActor;
  private readonly IRealmRepository _realmRepository;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public SetDefaultSenderHandler(ICurrentActor currentActor,
    IRealmRepository realmRepository,
    ISenderQuerier senderQuerier,
    ISenderRepository senderRepository)
  {
    _currentActor = currentActor;
    _realmRepository = realmRepository;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender> Handle(SetDefaultSender request, CancellationToken cancellationToken)
  {
    SenderAggregate sender = await _senderRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<SenderAggregate>(request.Id);

    if (!sender.IsDefault)
    {
      RealmAggregate realm = await _realmRepository.LoadAsync(sender, cancellationToken);

      SenderAggregate @default = await _senderRepository.LoadDefaultAsync(realm, cancellationToken)
        ?? throw new InvalidOperationException($"The default sender for realm '{realm}' could not be found.");

      @default.SetDefault(_currentActor.Id, isDefault: false);
      await _senderRepository.SaveAsync(@default, cancellationToken);

      sender.SetDefault(_currentActor.Id, isDefault: true);
      await _senderRepository.SaveAsync(sender, cancellationToken);
    }

    return await _senderQuerier.GetAsync(sender, cancellationToken);
  }
}
