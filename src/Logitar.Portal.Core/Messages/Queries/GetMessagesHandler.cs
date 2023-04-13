using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.Core.Messages.Queries;

internal class GetMessagesHandler : IRequestHandler<GetMessages, PagedList<Message>>
{
  private readonly IMessageQuerier _messageQuerier;

  public GetMessagesHandler(IMessageQuerier messageQuerier)
  {
    _messageQuerier = messageQuerier;
  }

  public async Task<PagedList<Message>> Handle(GetMessages request, CancellationToken cancellationToken)
  {
    return await _messageQuerier.GetAsync(request.HasErrors, request.IsDemo, request.Realm, request.Search, request.Succeeded,
      request.Template, request.Sort, request.IsDescending, request.Skip, request.Limit, cancellationToken);
  }
}
