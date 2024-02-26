using Logitar.Portal.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.Application.Messages.Queries;

internal class ReadMessageQueryHandler : IRequestHandler<ReadMessageQuery, Message?>
{
  private readonly IMessageQuerier _messageQuerier;

  public ReadMessageQueryHandler(IMessageQuerier messageQuerier)
  {
    _messageQuerier = messageQuerier;
  }

  public async Task<Message?> Handle(ReadMessageQuery query, CancellationToken cancellationToken)
  {
    return await _messageQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
