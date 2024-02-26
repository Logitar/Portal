using Logitar.Portal.Application.Senders.Commands;
using Logitar.Portal.Application.Senders.Queries;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders;

internal class SenderFacade : ISenderService
{
  private readonly IMediator _mediator;

  public SenderFacade(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<Sender> CreateAsync(CreateSenderPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new CreateSenderCommand(payload), cancellationToken);
  }

  public async Task<Sender?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new DeleteSenderCommand(id), cancellationToken);
  }

  public async Task<Sender?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReadSenderQuery(id), cancellationToken);
  }

  public async Task<Sender?> ReadDefaultAsync(CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReadDefaultSenderQuery(), cancellationToken);
  }

  public async Task<Sender?> ReplaceAsync(Guid id, ReplaceSenderPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReplaceSenderCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<Sender>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SearchSendersQuery(payload), cancellationToken);
  }

  public async Task<Sender?> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SetDefaultSenderCommand(id), cancellationToken);
  }

  public async Task<Sender?> UpdateAsync(Guid id, UpdateSenderPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new UpdateSenderCommand(id, payload), cancellationToken);
  }
}
