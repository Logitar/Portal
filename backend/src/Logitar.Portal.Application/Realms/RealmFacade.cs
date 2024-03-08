using Logitar.Portal.Application.Realms.Commands;
using Logitar.Portal.Application.Realms.Queries;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.Realms;

internal class RealmFacade : IRealmService
{
  private readonly IActivityPipeline _activityPipeline;

  public RealmFacade(IActivityPipeline activityPipeline)
  {
    _activityPipeline = activityPipeline;
  }

  public async Task<Realm> CreateAsync(CreateRealmPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new CreateRealmCommand(payload), cancellationToken);
  }

  public async Task<Realm?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new DeleteRealmCommand(id), cancellationToken);
  }

  public async Task<Realm?> ReadAsync(Guid? id, string? uniqueSlug, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReadRealmQuery(id, uniqueSlug), cancellationToken);
  }

  public async Task<Realm?> ReplaceAsync(Guid id, ReplaceRealmPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReplaceRealmCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SearchRealmsQuery(payload), cancellationToken);
  }

  public async Task<Realm?> UpdateAsync(Guid id, UpdateRealmPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new UpdateRealmCommand(id, payload), cancellationToken);
  }
}
