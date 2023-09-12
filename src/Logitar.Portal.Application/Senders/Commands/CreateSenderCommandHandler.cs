using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal class CreateSenderCommandHandler : IRequestHandler<CreateSenderCommand, Sender>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public CreateSenderCommandHandler(IApplicationContext applicationContext,
    IRealmRepository realmRepository, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender> Handle(CreateSenderCommand command, CancellationToken cancellationToken)
  {
    CreateSenderPayload payload = command.Payload;

    RealmAggregate? realm = null;
    if (payload.Realm != null)
    {
      realm = await _realmRepository.FindAsync(payload.Realm, cancellationToken)
        ?? throw new AggregateNotFoundException<RealmAggregate>(payload.Realm, nameof(payload.Realm));
    }
    string? tenantId = realm?.Id.Value;

    IEnumerable<SenderAggregate> senders = await _senderRepository.LoadAsync(tenantId, cancellationToken);
    bool isDefault = !senders.Any();

    SenderAggregate sender = new(payload.EmailAddress, payload.Provider, isDefault, tenantId, _applicationContext.ActorId)
    {
      DisplayName = payload.DisplayName,
      Description = payload.Description
    };

    foreach (ProviderSetting setting in payload.Settings)
    {
      if (setting.Value == null)
      {
        sender.RemoveSetting(setting.Key);
      }
      else
      {
        sender.SetSetting(setting.Key, setting.Value);
      }
    }

    sender.Update(_applicationContext.ActorId);

    await _senderRepository.SaveAsync(sender, cancellationToken);

    return await _senderQuerier.ReadAsync(sender, cancellationToken);
  }
}
