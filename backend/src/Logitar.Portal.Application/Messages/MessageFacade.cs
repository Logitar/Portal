using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Application.Messages.Queries;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.Messages;

internal class MessageFacade : IMessageService
{
  private readonly IActivityPipeline _activityPipeline;

  public MessageFacade(IActivityPipeline activityPipeline)
  {
    _activityPipeline = activityPipeline;
  }

  public async Task<MessageModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReadMessageQuery(id), cancellationToken);
  }

  public async Task<SearchResults<MessageModel>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SearchMessagesQuery(payload), cancellationToken);
  }

  public async Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SendMessageInternalCommand(payload), cancellationToken);
  }
}
