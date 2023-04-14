using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Senders;
using MediatR;

namespace Logitar.Portal.Core.Messages.Commands;

internal class ResolveSenderHandler : IRequestHandler<ResolveSender, SenderAggregate>
{
  private readonly ISenderRepository _senderRepository;

  public ResolveSenderHandler(ISenderRepository senderRepository)
  {
    _senderRepository = senderRepository;
  }

  public async Task<SenderAggregate> Handle(ResolveSender request, CancellationToken cancellationToken)
  {
    SenderAggregate sender;
    if (request.Id.HasValue)
    {
      sender = await _senderRepository.LoadAsync(request.Id.Value, cancellationToken)
        ?? throw new AggregateNotFoundException<SenderAggregate>(request.Id.Value, request.ParamName);
    }
    else
    {
      sender = await _senderRepository.LoadDefaultAsync(request.Realm, cancellationToken)
        ?? throw new DefaultSenderRequiredException(request.Realm);
    }

    if (request.Realm?.Id != sender.RealmId)
    {
      throw new SenderNotInRealmException(sender, request.Realm, request.ParamName);
    }

    return sender;
  }
}
