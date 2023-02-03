using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Application.Senders
{
  public interface ISenderService
  {
    Task<SenderModel> CreateAsync(CreateSenderPayload payload, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<SenderModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<SenderModel>> GetAsync(ProviderType? provider = null, string? realm = null, string? search = null,
      SenderSort? sort = null, bool isDescending = false, int? index = null, int? count = null, CancellationToken cancellationToken = default);
    Task<SenderModel?> GetDefaultAsync(string? realm = null, CancellationToken cancellationToken = default);
    Task<SenderModel> SetDefaultAsync(string id, CancellationToken cancellationToken = default);
    Task<SenderModel> UpdateAsync(string id, UpdateSenderPayload payload, CancellationToken cancellationToken = default);
  }
}
