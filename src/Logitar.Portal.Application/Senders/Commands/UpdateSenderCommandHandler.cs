using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal class UpdateSenderCommandHandler : IRequestHandler<UpdateSenderCommand, Sender?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public UpdateSenderCommandHandler(IApplicationContext applicationContext,
    ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender?> Handle(UpdateSenderCommand command, CancellationToken cancellationToken)
  {
    SenderAggregate? sender = await _senderRepository.LoadAsync(command.Id, cancellationToken);
    if (sender == null)
    {
      return null;
    }

    UpdateSenderPayload payload = command.Payload;

    if (payload.EmailAddress != null)
    {
      sender.EmailAddress = payload.EmailAddress;
    }
    if (payload.DisplayName != null)
    {
      sender.DisplayName = payload.DisplayName.Value;
    }
    if (payload.Description != null)
    {
      sender.Description = payload.Description.Value;
    }

    foreach (ProviderSettingModification setting in payload.Settings)
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
