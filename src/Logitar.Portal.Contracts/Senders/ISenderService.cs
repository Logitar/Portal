namespace Logitar.Portal.Contracts.Senders;

public interface ISenderService
{
  Task<Sender> CreateAsync(CreateSenderPayload payload, CancellationToken cancellationToken = default);
  Task<Sender?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Sender?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Sender?> ReadDefaultAsync(string? realm, CancellationToken cancellationToken = default);
  Task<Sender?> ReplaceAsync(Guid id, ReplaceSenderPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Sender>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken = default);
  Task<Sender?> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Sender?> UpdateAsync(Guid id, UpdateSenderPayload payload, CancellationToken cancellationToken = default);
}
