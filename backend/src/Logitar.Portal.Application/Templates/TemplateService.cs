using Logitar.Portal.Application.Templates.Commands;
using Logitar.Portal.Application.Templates.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.Application.Templates
{
  internal class TemplateService : ITemplateService
  {
    private readonly IRequestPipeline _requestPipeline;

    public TemplateService(IRequestPipeline requestPipeline)
    {
      _requestPipeline = requestPipeline;
    }

    public async Task<TemplateModel> CreateAsync(CreateTemplatePayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new CreateTemplateCommand(payload), cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
      await _requestPipeline.ExecuteAsync(new DeleteTemplateCommand(id), cancellationToken);
    }

    public async Task<TemplateModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetTemplateQuery(id), cancellationToken);
    }

    public async Task<ListModel<TemplateModel>> GetAsync(string? realm, string? search,
      TemplateSort? sort, bool isDescending, int? index, int? count, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetTemplatesQuery(realm, search,
        sort, isDescending, index, count), cancellationToken);
    }

    public async Task<TemplateModel> UpdateAsync(string id, UpdateTemplatePayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new UpdateTemplateCommand(id, payload), cancellationToken);
    }
  }
}
