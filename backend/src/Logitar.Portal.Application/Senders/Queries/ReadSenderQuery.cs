using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Queries;

internal record ReadSenderQuery(Guid Id) : Activity, IRequest<SenderModel?>;

internal class ReadSenderQueryHandler : IRequestHandler<ReadSenderQuery, SenderModel?>
{
  private readonly ISenderQuerier _senderQuerier;

  public ReadSenderQueryHandler(ISenderQuerier senderQuerier)
  {
    _senderQuerier = senderQuerier;
  }

  public async Task<SenderModel?> Handle(ReadSenderQuery query, CancellationToken cancellationToken)
  {
    return await _senderQuerier.ReadAsync(query.Realm, query.Id, cancellationToken);
  }
}
