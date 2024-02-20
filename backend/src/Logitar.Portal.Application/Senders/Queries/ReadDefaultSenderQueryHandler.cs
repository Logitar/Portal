using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Queries;

internal class ReadDefaultSenderQueryHandler : IRequestHandler<ReadDefaultSenderQuery, Sender?>
{
  private readonly ISenderQuerier _senderQuerier;

  public ReadDefaultSenderQueryHandler(ISenderQuerier senderQuerier)
  {
    _senderQuerier = senderQuerier;
  }

  public async Task<Sender?> Handle(ReadDefaultSenderQuery _, CancellationToken cancellationToken)
  {
    return await _senderQuerier.ReadDefaultAsync(cancellationToken);
  }
}
