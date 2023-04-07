using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Realms;
using Logitar.Portal.v2.Core.Realms.Commands;
using Logitar.Portal.v2.Core.Realms.Queries;

namespace Logitar.Portal.v2.Core.Realms;

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

  public async Task<Realm?> GetAsync(string idOrUniqueName, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new GetRealm(idOrUniqueName), cancellationToken);
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
