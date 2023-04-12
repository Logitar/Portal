using Logitar.Portal.v2.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.v2.Core.Senders.Commands;

internal class UpdateSenderHandler : IRequestHandler<UpdateSender, Sender>
{
  private readonly ICurrentActor _currentActor;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public UpdateSenderHandler(ICurrentActor currentActor,
    ISenderQuerier senderQuerier,
    ISenderRepository senderRepository)
  {
    _currentActor = currentActor;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender> Handle(UpdateSender request, CancellationToken cancellationToken)
  {
    SenderAggregate sender = await _senderRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<SenderAggregate>(request.Id);

    UpdateSenderInput input = request.Input;

    sender.Update(_currentActor.Id, input.EmailAddress, input.DisplayName, input.Settings?.ToDictionary());

    await _senderRepository.SaveAsync(sender, cancellationToken);

    return await _senderQuerier.GetAsync(sender, cancellationToken);
  }
}
