using Logitar.Portal.Application.Dictionaries.Commands;
using Logitar.Portal.Application.Dictionaries.Queries;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries;

internal class DictionaryFacade : IDictionaryService
{
  private readonly IMediator _mediator;

  public DictionaryFacade(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<Dictionary> CreateAsync(CreateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new CreateDictionaryCommand(payload), cancellationToken);
  }

  public async Task<Dictionary?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new DeleteDictionaryCommand(id), cancellationToken);
  }

  public async Task<Dictionary?> ReadAsync(Guid? id, string? locale, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReadDictionaryQuery(id, locale), cancellationToken);
  }

  public async Task<Dictionary?> ReplaceAsync(Guid id, ReplaceDictionaryPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReplaceDictionaryCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<Dictionary>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SearchDictionariesQuery(payload), cancellationToken);
  }

  public async Task<Dictionary?> UpdateAsync(Guid id, UpdateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new UpdateDictionaryCommand(id, payload), cancellationToken);
  }
}
