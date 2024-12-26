using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Application.Senders;

public interface ISenderService
{
  Task<SenderModel> CreateAsync(CreateSenderPayload payload, CancellationToken cancellationToken = default);
  Task<SenderModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SenderModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SenderModel?> ReadDefaultAsync(CancellationToken cancellationToken = default);
  Task<SenderModel?> ReplaceAsync(Guid id, ReplaceSenderPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<SearchResults<SenderModel>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken = default);
  Task<SenderModel?> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SenderModel?> UpdateAsync(Guid id, UpdateSenderPayload payload, CancellationToken cancellationToken = default);
}
