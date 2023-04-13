namespace Logitar.Portal.Contracts.Templates;

public interface ITemplateService
{
  Task<Template> CreateAsync(CreateTemplateInput input, CancellationToken cancellationToken = default);
  Task<Template> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Template?> GetAsync(Guid? id = null, string? realm = null, string? key = null, CancellationToken cancellationToken = default);
  Task<PagedList<Template>> GetAsync(string? realm = null, string? search = null,
    TemplateSort? sort = null, bool isDescending = false, int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
  Task<Template> UpdateAsync(Guid id, UpdateTemplateInput input, CancellationToken cancellationToken = default);
}
