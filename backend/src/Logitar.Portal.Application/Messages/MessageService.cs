using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Application.Messages.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Application.Messages
{
  internal class MessageService : IMessageService
  {
    private readonly IRequestPipeline _requestPipeline;

    public MessageService(IRequestPipeline requestPipeline)
    {
      _requestPipeline = requestPipeline;
    }

    public async Task<MessageModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetMessageQuery(id), cancellationToken);
    }

    public async Task<ListModel<MessageModel>> GetAsync(bool? hasErrors, bool? hasSucceeded, bool? isDemo, string? realm, string? search, string? template,
      MessageSort? sort, bool isDescending,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetMessagesQuery(hasErrors, hasSucceeded, isDemo, realm, search, template,
        sort, isDescending,
        index, count), cancellationToken);
    }

    public async Task<SentMessagesModel> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new SendMessageCommand(payload), cancellationToken);
    }

    public async Task<MessageModel> SendDemoAsync(SendDemoMessagePayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new SendDemoMessageCommand(payload), cancellationToken);
    }
  }
}
