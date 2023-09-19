using Logitar.Portal.Application.Senders.Commands;
using Logitar.Portal.Application.Senders.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Application.Senders;

internal class SenderService : ISenderService
{
  private readonly IRequestPipeline _pipeline;

  public SenderService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<Sender> CreateAsync(CreateSenderPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateSenderCommand(payload), cancellationToken);
  }

  public async Task<Sender?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new DeleteSenderCommand(id), cancellationToken);
  }

  public async Task<Sender?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReadSenderQuery(id), cancellationToken);
  }

  public async Task<Sender?> ReadDefaultAsync(string? realm, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReadDefaultSenderQuery(realm), cancellationToken);
  }

  public async Task<Sender?> ReplaceAsync(Guid id, ReplaceSenderPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReplaceSenderCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<Sender>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SearchSendersQuery(payload), cancellationToken);
  }

  public async Task<Sender?> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SetDefaultSenderCommand(id), cancellationToken);
  }

  public async Task<Sender?> UpdateAsync(Guid id, UpdateSenderPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new UpdateSenderCommand(id, payload), cancellationToken);
  }
}
