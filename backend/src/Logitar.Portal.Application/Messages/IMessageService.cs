using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Application.Messages
{
  public interface IMessageService
  {
    Task<MessageModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<MessageModel>> GetAsync(bool? hasErrors = null, bool? hasSucceeded = null, bool? isDemo = null, string? realm = null, string? search = null, string? template = null,
      MessageSort? sort = null, bool isDescending = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
    Task<SentMessagesModel> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken = default);
    Task<MessageModel> SendDemoAsync(SendDemoMessagePayload payload, CancellationToken cancellationToken = default);
  }
}
