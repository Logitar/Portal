using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain;
using MediatR;

namespace Logitar.Portal.Application.Messages.Queries
{
  internal class GetMessageQueryHandler : IRequestHandler<GetMessageQuery, MessageModel?>
  {
    private readonly IMessageQuerier _messageQuerier;

    public GetMessageQueryHandler(IMessageQuerier messageQuerier)
    {
      _messageQuerier = messageQuerier;
    }

    public async Task<MessageModel?> Handle(GetMessageQuery request, CancellationToken cancellationToken)
    {
      return await _messageQuerier.GetAsync(new AggregateId(request.Id), cancellationToken);
    }
  }
}
