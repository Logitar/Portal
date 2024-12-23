using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Senders;

public interface ISenderClient
{
  Task<SenderModel> CreateAsync(CreateSenderPayload payload, IRequestContext? context = null);
  Task<SenderModel?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<SenderModel?> ReadAsync(Guid id, IRequestContext? context = null);
  Task<SenderModel?> ReadDefaultAsync(IRequestContext? context = null);
  Task<SenderModel?> ReplaceAsync(Guid id, ReplaceSenderPayload payload, long? version = null, IRequestContext? context = null);
  Task<SearchResults<SenderModel>> SearchAsync(SearchSendersPayload payload, IRequestContext? context = null);
  Task<SenderModel?> SetDefaultAsync(Guid id, IRequestContext? context = null);
  Task<SenderModel?> UpdateAsync(Guid id, UpdateSenderPayload payload, IRequestContext? context = null);
}
