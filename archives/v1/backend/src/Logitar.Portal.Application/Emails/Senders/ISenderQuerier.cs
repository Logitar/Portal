using Logitar.Portal.Core.Emails.Senders;
using Logitar.Portal.Domain.Emails.Senders;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Emails.Senders
{
  public interface ISenderQuerier
  {
    Task<Sender?> GetAsync(Guid id, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<Sender?> GetDefaultAsync(Realm? realm = null, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<PagedList<Sender>> GetPagedAsync(ProviderType? provider = null, string? realm = null, string? search = null,
      SenderSort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      bool readOnly = false, CancellationToken cancellationToken = default);
  }
}
