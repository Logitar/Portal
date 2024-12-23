using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Templates.Commands;
using Logitar.Portal.Application.Templates.Queries;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.Application.Templates;

internal class TemplateFacade : ITemplateService
{
  private readonly IActivityPipeline _activityPipeline;

  public TemplateFacade(IActivityPipeline activityPipeline)
  {
    _activityPipeline = activityPipeline;
  }

  public async Task<TemplateModel> CreateAsync(CreateTemplatePayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new CreateTemplateCommand(payload), cancellationToken);
  }

  public async Task<TemplateModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new DeleteTemplateCommand(id), cancellationToken);
  }

  public async Task<TemplateModel?> ReadAsync(Guid? id, string? uniqueKey, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReadTemplateQuery(id, uniqueKey), cancellationToken);
  }

  public async Task<TemplateModel?> ReplaceAsync(Guid id, ReplaceTemplatePayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReplaceTemplateCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<TemplateModel>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SearchTemplatesQuery(payload), cancellationToken);
  }

  public async Task<TemplateModel?> UpdateAsync(Guid id, UpdateTemplatePayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new UpdateTemplateCommand(id, payload), cancellationToken);
  }
}
