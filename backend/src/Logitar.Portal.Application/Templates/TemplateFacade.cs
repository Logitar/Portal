using Logitar.Portal.Application.Templates.Commands;
using Logitar.Portal.Application.Templates.Queries;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates;

internal class TemplateFacade : ITemplateService
{
  private readonly IMediator _mediator;

  public TemplateFacade(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<Template> CreateAsync(CreateTemplatePayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new CreateTemplateCommand(payload), cancellationToken);
  }

  public async Task<Template?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new DeleteTemplateCommand(id), cancellationToken);
  }

  public async Task<Template?> ReadAsync(Guid? id, string? uniqueKey, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReadTemplateQuery(id, uniqueKey), cancellationToken);
  }

  public async Task<Template?> ReplaceAsync(Guid id, ReplaceTemplatePayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReplaceTemplateCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<Template>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SearchTemplatesQuery(payload), cancellationToken);
  }

  public async Task<Template?> UpdateAsync(Guid id, UpdateTemplatePayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new UpdateTemplateCommand(id, payload), cancellationToken);
  }
}
