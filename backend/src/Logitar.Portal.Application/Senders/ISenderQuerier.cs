using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Senders
{
  public interface ISenderQuerier
  {
    Task<SenderModel?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
    Task<SenderModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<SenderModel?> GetDefaultAsync(Realm? realm, CancellationToken cancellationToken = default);
    Task<ListModel<SenderModel>> GetPagedAsync(ProviderType? provider = null, string? realm = null, string? search = null,
      SenderSort? sort = null, bool isDescending = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
  }
}
