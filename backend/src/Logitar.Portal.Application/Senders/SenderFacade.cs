using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Senders.Commands;
using Logitar.Portal.Application.Senders.Queries;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Application.Senders;

internal class SenderFacade : ISenderService
{
  private readonly IActivityPipeline _activityPipeline;

  public SenderFacade(IActivityPipeline activityPipeline)
  {
    _activityPipeline = activityPipeline;
  }

  public async Task<SenderModel> CreateAsync(CreateSenderPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new CreateSenderCommand(payload), cancellationToken);
  }

  public async Task<SenderModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new DeleteSenderCommand(id), cancellationToken);
  }

  public async Task<SenderModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReadSenderQuery(id), cancellationToken);
  }

  public async Task<SenderModel?> ReadDefaultAsync(CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReadDefaultSenderQuery(), cancellationToken);
  }

  public async Task<SenderModel?> ReplaceAsync(Guid id, ReplaceSenderPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReplaceSenderCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<SenderModel>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SearchSendersQuery(payload), cancellationToken);
  }

  public async Task<SenderModel?> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SetDefaultSenderCommand(id), cancellationToken);
  }

  public async Task<SenderModel?> UpdateAsync(Guid id, UpdateSenderPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new UpdateSenderCommand(id, payload), cancellationToken);
  }
}
