using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.v2.Core.Messages.Queries;

internal class GetMessageHandler : IRequestHandler<GetMessage, Message?>
{
  private readonly IMessageQuerier _messageQuerier;

  public GetMessageHandler(IMessageQuerier messageQuerier)
  {
    _messageQuerier = messageQuerier;
  }

  public async Task<Message?> Handle(GetMessage request, CancellationToken cancellationToken)
  {
    List<Message> messages = new(capacity: 1);

    if (request.Id.HasValue)
    {
      messages.AddIfNotNull(await _messageQuerier.GetAsync(request.Id.Value, cancellationToken));
    }

    if (messages.Count > 1)
    {
      throw new TooManyResultsException();
    }

    return messages.SingleOrDefault();
  }
}
