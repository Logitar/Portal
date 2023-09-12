using Logitar.Portal.Application.Dictionaries.Commands;
using Logitar.Portal.Application.Dictionaries.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries;

internal class DictionaryService : IDictionaryService
{
  private readonly IRequestPipeline _pipeline;

  public DictionaryService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<Dictionary> CreateAsync(CreateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateDictionaryCommand(payload), cancellationToken);
  }

  public async Task<Dictionary?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new DeleteDictionaryCommand(id), cancellationToken);
  }

  public async Task<Dictionary?> ReadAsync(Guid? id, string? realm, string? locale, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReadDictionaryQuery(id, realm, locale), cancellationToken);
  }

  public async Task<Dictionary?> ReplaceAsync(Guid id, ReplaceDictionaryPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReplaceDictionaryCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<Dictionary>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SearchDictionariesQuery(payload), cancellationToken);
  }

  public async Task<Dictionary?> UpdateAsync(Guid id, UpdateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new UpdateDictionaryCommand(id, payload), cancellationToken);
  }
}
