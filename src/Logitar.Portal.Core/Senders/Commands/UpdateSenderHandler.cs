using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Core.Senders.Commands;

internal class UpdateSenderHandler : IRequestHandler<UpdateSender, Sender>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public UpdateSenderHandler(IApplicationContext applicationContext,
    ISenderQuerier senderQuerier,
    ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender> Handle(UpdateSender request, CancellationToken cancellationToken)
  {
    SenderAggregate sender = await _senderRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<SenderAggregate>(request.Id);

    UpdateSenderInput input = request.Input;

    sender.Update(_applicationContext.ActorId, input.EmailAddress, input.DisplayName, input.Settings?.ToDictionary());

    await _senderRepository.SaveAsync(sender, cancellationToken);

    return await _senderQuerier.GetAsync(sender, cancellationToken);
  }
}
