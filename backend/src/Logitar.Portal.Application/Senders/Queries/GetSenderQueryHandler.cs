using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Queries
{
  internal class GetSenderQueryHandler : IRequestHandler<GetSenderQuery, SenderModel?>
  {
    private readonly ISenderQuerier _senderQuerier;

    public GetSenderQueryHandler(ISenderQuerier senderQuerier)
    {
      _senderQuerier = senderQuerier;
    }

    public async Task<SenderModel?> Handle(GetSenderQuery request, CancellationToken cancellationToken)
    {
      return await _senderQuerier.GetAsync(request.Id, cancellationToken);
    }
  }
}
