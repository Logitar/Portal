using Portal.Core.Emails.Senders.Models;
using Portal.Core.Emails.Senders.Payloads;

namespace Portal.Core.Emails.Senders
{
  public interface ISenderService
  {
    Task<SenderModel> CreateAsync(CreateSenderPayload payload, CancellationToken cancellationToken = default);
    Task<SenderModel> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SenderModel?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ListModel<SenderModel>> GetAsync(ProviderType? provider = null, string? realm = null, string? search = null,
      SenderSort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
    Task<SenderModel?> GetDefaultAsync(string? realm = null, CancellationToken cancellationToken = default);
    Task<SenderModel> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SenderModel> UpdateAsync(Guid id, UpdateSenderPayload payload, CancellationToken cancellationToken = default);
  }
}
