using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal class ReplaceSenderCommandHandler : IRequestHandler<ReplaceSenderCommand, Sender?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public ReplaceSenderCommandHandler(IApplicationContext applicationContext,
    ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender?> Handle(ReplaceSenderCommand command, CancellationToken cancellationToken)
  {
    SenderAggregate? sender = await _senderRepository.LoadAsync(command.Id, cancellationToken);
    if (sender == null)
    {
      return null;
    }

    SenderAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _senderRepository.LoadAsync(sender.Id, command.Version.Value, cancellationToken);
    }

    ReplaceSenderPayload payload = command.Payload;

    if (reference == null || payload.EmailAddress.Trim() != reference.EmailAddress)
    {
      sender.EmailAddress = payload.EmailAddress;
    }
    if (reference == null || payload.DisplayName?.CleanTrim() != reference.DisplayName)
    {
      sender.DisplayName = payload.DisplayName;
    }
    if (reference == null || payload.Description?.CleanTrim() != reference.Description)
    {
      sender.Description = payload.Description;
    }

    HashSet<string> settingKeys = payload.Settings.Select(x => x.Key.Trim()).ToHashSet();
    foreach (string key in sender.Settings.Keys)
    {
      if (!settingKeys.Contains(key) && (reference == null || reference.Settings.ContainsKey(key)))
      {
        sender.RemoveSetting(key);
      }
    }
    foreach (ProviderSetting providerSetting in payload.Settings)
    {
      sender.SetSetting(providerSetting.Key, providerSetting.Value);
    }

    sender.Update(_applicationContext.ActorId);

    await _senderRepository.SaveAsync(sender, cancellationToken);

    return await _senderQuerier.ReadAsync(sender, cancellationToken);
  }
}
