using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Application.Messages.Queries;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.Messages;

internal class MessageFacade : IMessageService
{
  private readonly IMediator _mediator;

  public MessageFacade(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<Message?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReadMessageQuery(id), cancellationToken);
  }

  public async Task<SearchResults<Message>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SearchMessagesQuery(payload), cancellationToken);
  }

  public async Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SendMessageCommand(payload), cancellationToken);
  }
}
