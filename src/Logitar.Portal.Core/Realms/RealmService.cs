using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Core.Realms.Commands;
using Logitar.Portal.Core.Realms.Queries;

namespace Logitar.Portal.Core.Realms;

internal class RealmService : IRealmService
{
  private readonly IRequestPipeline _pipeline;

  public RealmService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<Realm> CreateAsync(CreateRealmInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateRealm(input), cancellationToken);
  }

  public async Task<Realm> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new DeleteRealm(id), cancellationToken);
  }

  public async Task<Realm?> GetAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new GetRealm(id, uniqueName), cancellationToken);
  }

  public async Task<PagedList<Realm>> GetAsync(string? search, RealmSort? sort, bool isDescending,
    int? skip, int? limit, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new GetRealms(search, sort, isDescending, skip, limit), cancellationToken);
  }

  public async Task<Realm> UpdateAsync(Guid id, UpdateRealmInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new UpdateRealm(id, input), cancellationToken);
  }
}
