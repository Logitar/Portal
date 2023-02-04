using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain;

namespace Logitar.Portal.Application.Messages
{
  public interface IMessageQuerier
  {
    Task<MessageModel?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
    Task<ListModel<MessageModel>> GetPagedAsync(bool? hasErrors = null, bool? hasSucceeded = null, bool? isDemo = null, string? realm = null, string? search = null, string? template = null,
      MessageSort? sort = null, bool isDescending = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
  }
}
