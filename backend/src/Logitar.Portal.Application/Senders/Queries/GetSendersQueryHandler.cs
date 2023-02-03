using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Queries
{
  internal class GetSendersQueryHandler : IRequestHandler<GetSendersQuery, ListModel<SenderModel>>
  {
    private readonly ISenderQuerier _senderQuerier;

    public GetSendersQueryHandler(ISenderQuerier senderQuerier)
    {
      _senderQuerier = senderQuerier;
    }

    public async Task<ListModel<SenderModel>> Handle(GetSendersQuery request, CancellationToken cancellationToken)
    {
      return await _senderQuerier.GetPagedAsync(request.Provider, request.Realm, request.Search,
        request.Sort, request.IsDescending,
        request.Index, request.Count,
        cancellationToken);
    }
  }
}
