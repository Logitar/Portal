using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Templates;

namespace Logitar.Portal.v2.Core.Templates;

public interface ITemplateQuerier
{
  Task<Template> GetAsync(TemplateAggregate template, CancellationToken cancellationToken = default);
  Task<Template?> GetAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Template?> GetAsync(string realm, string key, CancellationToken cancellationToken = default);
  Task<PagedList<Template>> GetAsync(string? realm = null, string? search = null, TemplateSort? sort = null,
  bool isDescending = false, int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
}
