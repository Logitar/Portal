using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Dictionaries.Commands;
using Logitar.Portal.Application.Dictionaries.Queries;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.Dictionaries;

internal class DictionaryFacade : IDictionaryService
{
  private readonly IActivityPipeline _activityPipeline;

  public DictionaryFacade(IActivityPipeline activityPipeline)
  {
    _activityPipeline = activityPipeline;
  }

  public async Task<Dictionary> CreateAsync(CreateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new CreateDictionaryCommand(payload), cancellationToken);
  }

  public async Task<Dictionary?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new DeleteDictionaryCommand(id), cancellationToken);
  }

  public async Task<Dictionary?> ReadAsync(Guid? id, string? locale, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReadDictionaryQuery(id, locale), cancellationToken);
  }

  public async Task<Dictionary?> ReplaceAsync(Guid id, ReplaceDictionaryPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReplaceDictionaryCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<Dictionary>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SearchDictionariesQuery(payload), cancellationToken);
  }

  public async Task<Dictionary?> UpdateAsync(Guid id, UpdateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new UpdateDictionaryCommand(id, payload), cancellationToken);
  }
}
