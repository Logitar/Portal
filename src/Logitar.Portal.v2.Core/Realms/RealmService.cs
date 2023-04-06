using Logitar.Portal.v2.Contracts.Realms;
using Logitar.Portal.v2.Core.Realms.Commands;

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

  public async Task<Realm> UpdateAsync(Guid id, UpdateRealmInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new UpdateRealm(id, input), cancellationToken);
  }
}
