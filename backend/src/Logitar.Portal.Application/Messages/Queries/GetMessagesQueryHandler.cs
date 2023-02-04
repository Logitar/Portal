using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.Application.Messages.Queries
{
  internal class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, ListModel<MessageModel>>
  {
    private readonly IMessageQuerier _messageQuerier;

    public GetMessagesQueryHandler(IMessageQuerier messageQuerier)
    {
      _messageQuerier = messageQuerier;
    }

    public async Task<ListModel<MessageModel>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
      return await _messageQuerier.GetPagedAsync(request.HasErrors, request.HasSucceeded, request.IsDemo, request.Realm, request.Search, request.Template,
        request.Sort, request.IsDecending,
        request.Index, request.Count,
        cancellationToken);
    }
  }
}
