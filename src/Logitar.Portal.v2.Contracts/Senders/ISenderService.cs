namespace Logitar.Portal.v2.Contracts.Senders;

public interface ISenderService
{
  Task<Sender> CreateAsync(CreateSenderInput input, CancellationToken cancellationToken = default);
  Task<Sender> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Sender?> GetAsync(Guid? id = null, string? defaultInRealm = null, CancellationToken cancellationToken = default);
  Task<PagedList<Sender>> GetAsync(ProviderType? provider = null, string? realm = null, string? search = null,
    SenderSort? sort = null, bool isDescending = false, int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
  Task<Sender> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Sender> UpdateAsync(Guid id, UpdateSenderInput input, CancellationToken cancellationToken = default);
}
