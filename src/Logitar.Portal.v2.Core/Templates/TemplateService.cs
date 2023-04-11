using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Templates;
using Logitar.Portal.v2.Core.Templates.Commands;
using Logitar.Portal.v2.Core.Templates.Queries;

namespace Logitar.Portal.v2.Core.Templates;

internal class TemplateService : ITemplateService
{
  private readonly IRequestPipeline _pipeline;

  public TemplateService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<Template> CreateAsync(CreateTemplateInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateTemplate(input), cancellationToken);
  }

  public async Task<Template> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new DeleteTemplate(id), cancellationToken);
  }

  public async Task<Template?> GetAsync(Guid? id, string? realm, string? key, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new GetTemplate(id, realm, key), cancellationToken);
  }

  public async Task<PagedList<Template>> GetAsync(string? realm, string? search, TemplateSort? sort,
    bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new GetTemplates(realm, search, sort, isDescending, skip, limit), cancellationToken);
  }

  public async Task<Template> UpdateAsync(Guid id, UpdateTemplateInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new UpdateTemplate(id, input), cancellationToken);
  }
}
