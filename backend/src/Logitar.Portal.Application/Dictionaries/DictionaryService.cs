using Logitar.Portal.Application.Dictionaries.Commands;
using Logitar.Portal.Application.Dictionaries.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries
{
  internal class DictionaryService : IDictionaryService
  {
    private readonly IRequestPipeline _requestPipeline;

    public DictionaryService(IRequestPipeline requestPipeline)
    {
      _requestPipeline = requestPipeline;
    }

    public async Task<DictionaryModel> CreateAsync(CreateDictionaryPayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new CreateDictionaryCommand(payload), cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
      await _requestPipeline.ExecuteAsync(new DeleteDictionaryCommand(id), cancellationToken);
    }

    public async Task<DictionaryModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetDictionaryQuery(id), cancellationToken);
    }

    public async Task<ListModel<DictionaryModel>> GetAsync(string? locale, string? realm,
      DictionarySort? sort, bool isDescending, int? index, int? count, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetDictionariesQuery(locale, realm,
        sort, isDescending, index, count), cancellationToken);
    }

    public async Task<DictionaryModel> UpdateAsync(string id, UpdateDictionaryPayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new UpdateDictionaryCommand(id, payload), cancellationToken);
    }
  }
}
