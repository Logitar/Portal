using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Senders.Validators;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.SendGrid;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal class CreateSenderCommandHandler : IRequestHandler<CreateSenderCommand, Sender>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public CreateSenderCommandHandler(IApplicationContext applicationContext, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender> Handle(CreateSenderCommand command, CancellationToken cancellationToken)
  {
    CreateSenderPayload payload = command.Payload;
    new CreateSenderValidator().ValidateAndThrow(payload);

    ActorId actorId = _applicationContext.ActorId;

    EmailUnit email = payload.Email.ToEmailUnit();
    ReadOnlySendGridSettings settings = new(payload.SendGrid);
    SenderAggregate sender = new(email, settings, _applicationContext.TenantId, actorId)
    {
      DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName),
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    sender.Update(actorId);

    IEnumerable<SenderAggregate> senders = await _senderRepository.LoadAsync(sender.TenantId, cancellationToken);
    if (!senders.Any())
    {
      sender.SetDefault(actorId);
    }

    await _senderRepository.SaveAsync(sender, cancellationToken);

    return await _senderQuerier.ReadAsync(sender, cancellationToken);
  }
}
