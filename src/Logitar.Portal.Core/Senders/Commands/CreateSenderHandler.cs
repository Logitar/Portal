using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Core.Realms;
using MediatR;

namespace Logitar.Portal.Core.Senders.Commands;

internal class CreateSenderHandler : IRequestHandler<CreateSender, Sender>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public CreateSenderHandler(IApplicationContext applicationContext,
    IRealmRepository realmRepository,
    ISenderQuerier senderQuerier,
    ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender> Handle(CreateSender request, CancellationToken cancellationToken)
  {
    CreateSenderInput input = request.Input;

    RealmAggregate? realm = await LoadRealmAsync(input, cancellationToken);

    SenderAggregate sender = new(_applicationContext.ActorId, realm, input.Provider, input.EmailAddress,
      input.DisplayName, input.Settings?.ToDictionary());
    if (await _senderRepository.LoadDefaultAsync(realm, cancellationToken) == null)
    {
      sender.SetDefault(_applicationContext.ActorId);
    }

    await _senderRepository.SaveAsync(sender, cancellationToken);

    return await _senderQuerier.GetAsync(sender, cancellationToken);
  }

  private async Task<RealmAggregate?> LoadRealmAsync(CreateSenderInput input, CancellationToken cancellationToken)
  {
    if (input.Realm == null)
    {
      return null;
    }

    return await _realmRepository.LoadAsync(input.Realm, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(input.Realm, nameof(input.Realm));
  }
}
