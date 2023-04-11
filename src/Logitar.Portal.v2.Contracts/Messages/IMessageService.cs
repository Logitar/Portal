namespace Logitar.Portal.v2.Contracts.Messages;

public interface IMessageService
{
  Task<Message?> GetAsync(Guid? id = null, CancellationToken cancellationToken = default);
  Task<PagedList<Message>> GetAsync(bool? hasErrors = null, bool? isDemo = null, string? realm = null, string? search = null, bool? succeeded = null, string? template = null,
    MessageSort? sort = null, bool isDescending = false, int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
  Task<SentMessages> SendAsync(SendMessageInput input, CancellationToken cancellationToken = default);
}
