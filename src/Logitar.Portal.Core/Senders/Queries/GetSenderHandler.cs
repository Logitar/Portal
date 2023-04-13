using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Core.Senders.Queries;

internal class GetSenderHandler : IRequestHandler<GetSender, Sender?>
{
  private readonly ISenderQuerier _senderQuerier;

  public GetSenderHandler(ISenderQuerier senderQuerier)
  {
    _senderQuerier = senderQuerier;
  }

  public async Task<Sender?> Handle(GetSender request, CancellationToken cancellationToken)
  {
    List<Sender> senders = new(capacity: 2);

    if (request.Id.HasValue)
    {
      senders.AddIfNotNull(await _senderQuerier.GetAsync(request.Id.Value, cancellationToken));
    }
    if (request.DefaultInRealm != null)
    {
      senders.AddIfNotNull(await _senderQuerier.GetDefaultAsync(request.DefaultInRealm, cancellationToken));
    }

    if (senders.Count > 1)
    {
      throw new TooManyResultsException();
    }

    return senders.SingleOrDefault();
  }
}
