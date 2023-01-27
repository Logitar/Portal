using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Realms.Commands;
using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Core.Realms.Payloads;
using Logitar.Portal.Core.Realms.Queries;

namespace Logitar.Portal.Core.Realms
{
  internal class RealmService : IRealmService
  {
    private readonly IRequestPipeline _requestPipeline;

    public RealmService(IRequestPipeline requestPipeline)
    {
      _requestPipeline = requestPipeline;
    }

    public async Task<RealmModel> CreateAsync(CreateRealmPayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new CreateRealmCommand(payload), cancellationToken);
    }

    public async Task<RealmModel> DeleteAsync(string id, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new DeleteRealmCommand(id), cancellationToken);
    }

    public async Task<RealmModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetRealmQuery(id), cancellationToken);
    }

    public async Task<ListModel<RealmModel>> GetAsync(string? search,
      RealmSort? sort, bool isDescending,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetRealmsQuery
      {
        Search = search,
        Sort = sort,
        IsDescending = isDescending,
        Index = index,
        Count = count
      }, cancellationToken);
    }

    public async Task<RealmModel> UpdateAsync(string id, UpdateRealmPayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new UpdateRealmCommand(id, payload), cancellationToken);
    }
  }
}
