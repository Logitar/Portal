using Logitar.Portal.Application.Realms.Commands;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Application.Realms;

internal class RealmService : IRealmService
{
  private readonly IRequestPipeline _pipeline;

  public RealmService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<Realm> CreateAsync(CreateRealmPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateRealmCommand(payload), cancellationToken);
  }

  public async Task<Realm?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new DeleteRealmCommand(id), cancellationToken);
  }

  public Task<Realm?> ReadAsync(Guid? id, string? uniqueSlug, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<Realm?> ReplaceAsync(Guid id, ReplaceRealmPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<Realm?> UpdateAsync(Guid id, UpdateRealmPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
