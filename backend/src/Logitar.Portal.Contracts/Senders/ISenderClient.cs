using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Senders;

public interface ISenderClient
{
  Task<Sender> CreateAsync(CreateSenderPayload payload, IRequestContext? context = null);
  Task<Sender?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<Sender?> ReadAsync(Guid id, IRequestContext? context = null);
  Task<Sender?> ReadDefaultAsync(IRequestContext? context = null);
  Task<Sender?> ReplaceAsync(Guid id, ReplaceSenderPayload payload, long? version = null, IRequestContext? context = null);
  Task<SearchResults<Sender>> SearchAsync(SearchSendersPayload payload, IRequestContext? context = null);
  Task<Sender?> SetDefaultAsync(Guid id, IRequestContext? context = null);
  Task<Sender?> UpdateAsync(Guid id, UpdateSenderPayload payload, IRequestContext? context = null);
}
