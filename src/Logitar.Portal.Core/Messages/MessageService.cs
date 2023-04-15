using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Core.Messages.Commands;
using Logitar.Portal.Core.Messages.Queries;

namespace Logitar.Portal.Core.Messages;

internal class MessageService : IMessageService
{
  private readonly IRequestPipeline _pipeline;

  public MessageService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<Message?> GetAsync(Guid? id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new GetMessage(id), cancellationToken);
  }

  public async Task<PagedList<Message>> GetAsync(bool? hasErrors, bool? isDemo, string? realm, string? search, bool? succeeded, string? template,
    MessageSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new GetMessages(hasErrors, isDemo, realm, search, succeeded, template,
      sort, isDescending, skip, limit), cancellationToken);
  }

  public async Task<SentMessages> SendAsync(SendMessageInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SendMessage(input), cancellationToken);
  }

  public async Task<Message> SendDemoAsync(SendDemoMessageInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SendDemoMessage(input), cancellationToken);
  }
}
