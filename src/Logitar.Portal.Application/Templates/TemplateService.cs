using Logitar.Portal.Application.Templates.Commands;
using Logitar.Portal.Application.Templates.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.Application.Templates;

internal class TemplateService : ITemplateService
{
  private readonly IRequestPipeline _pipeline;

  public TemplateService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<Template> CreateAsync(CreateTemplatePayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateTemplateCommand(payload), cancellationToken);
  }

  public async Task<Template?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new DeleteTemplateCommand(id), cancellationToken);
  }

  public async Task<Template?> ReadAsync(Guid? id, string? realm, string? uniqueName, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReadTemplateQuery(id, realm, uniqueName), cancellationToken);
  }

  public async Task<Template?> ReplaceAsync(Guid id, ReplaceTemplatePayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReplaceTemplateCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<Template>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SearchTemplatesQuery(payload), cancellationToken);
  }

  public async Task<Template?> UpdateAsync(Guid id, UpdateTemplatePayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new UpdateTemplateCommand(id, payload), cancellationToken);
  }
}
