using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Messages;

namespace Logitar.Portal.v2.Core.Messages;

public interface IMessageQuerier
{
  Task<Message> GetAsync(MessageAggregate message, CancellationToken cancellationToken = default);
  Task<Message?> GetAsync(Guid id, CancellationToken cancellationToken = default);
  Task<PagedList<Message>> GetAsync(bool? hasErrors = null, bool? isDemo = null, string? realm = null, string? search = null, bool? succeeded = null, string? template = null,
    MessageSort? sort = null, bool isDescending = false, int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
}
