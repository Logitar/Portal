using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Core.Senders.Queries;

internal class GetSendersHandler : IRequestHandler<GetSenders, PagedList<Sender>>
{
  private readonly ISenderQuerier _senderQuerier;

  public GetSendersHandler(ISenderQuerier senderQuerier)
  {
    _senderQuerier = senderQuerier;
  }

  public async Task<PagedList<Sender>> Handle(GetSenders request, CancellationToken cancellationToken)
  {
    return await _senderQuerier.GetAsync(request.Provider, request.Realm, request.Search,
      request.Sort, request.IsDescending, request.Skip, request.Limit, cancellationToken);
  }
}
