using Logitar.Portal.Core;
using Logitar.Portal.Core.Emails.Messages;
using Logitar.Portal.Core.Emails.Messages.Models;
using Logitar.Portal.Core.Emails.Messages.Payloads;

namespace Logitar.Portal.Client
{
  public interface IMessageService
  {
    Task<MessageModel> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ListModel<MessageSummary>> GetAsync(bool? hasErrors = null, bool? isDemo = null, string? realm = null, string? search = null, bool? succeeded = null, string? template = null,
      MessageSort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
    Task<SentMessagesModel> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken = default);
  }
}
