using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Dictionaries;
using Logitar.Portal.v2.Core.Dictionaries.Commands;
using Logitar.Portal.v2.Core.Dictionaries.Queries;

namespace Logitar.Portal.v2.Core.Dictionaries;

internal class DictionaryService : IDictionaryService
{
  private readonly IRequestPipeline _pipeline;

  public DictionaryService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<Dictionary> CreateAsync(CreateDictionaryInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateDictionary(input), cancellationToken);
  }

  public async Task<Dictionary> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new DeleteDictionary(id), cancellationToken);
  }

  public async Task<Dictionary?> GetAsync(Guid? id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new GetDictionary(id), cancellationToken);
  }

  public async Task<PagedList<Dictionary>> GetAsync(string? locale, string? realm, DictionarySort? sort,
    bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new GetDictionaries(locale, realm, sort, isDescending, skip, limit), cancellationToken);
  }

  public async Task<Dictionary> UpdateAsync(Guid id, UpdateDictionaryInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new UpdateDictionary(id, input), cancellationToken);
  }
}
