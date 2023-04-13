using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Core.Realms;
using MediatR;

namespace Logitar.Portal.Core.Senders.Commands;

internal class CreateSenderHandler : IRequestHandler<CreateSender, Sender>
{
  private readonly ICurrentActor _currentActor;
  private readonly IRealmRepository _realmRepository;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public CreateSenderHandler(ICurrentActor currentActor,
    IRealmRepository realmRepository,
    ISenderQuerier senderQuerier,
    ISenderRepository senderRepository)
  {
    _currentActor = currentActor;
    _realmRepository = realmRepository;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender> Handle(CreateSender request, CancellationToken cancellationToken)
  {
    CreateSenderInput input = request.Input;

    RealmAggregate realm = await _realmRepository.LoadAsync(input.Realm, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(input.Realm, nameof(input.Realm));

    SenderAggregate sender = new(_currentActor.Id, realm, input.Provider, input.EmailAddress,
      input.DisplayName, input.Settings?.ToDictionary());
    if (await _senderRepository.LoadDefaultAsync(realm, cancellationToken) == null)
    {
      sender.SetDefault(_currentActor.Id);
    }

    await _senderRepository.SaveAsync(sender, cancellationToken);

    return await _senderQuerier.GetAsync(sender, cancellationToken);
  }
}
