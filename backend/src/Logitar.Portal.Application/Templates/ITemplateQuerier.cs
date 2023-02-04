using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain;

namespace Logitar.Portal.Application.Templates
{
  public interface ITemplateQuerier
  {
    Task<TemplateModel?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
    Task<TemplateModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<TemplateModel>> GetPagedAsync(string? realm = null, string? search = null,
      TemplateSort? sort = null, bool isDescending = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
  }
}
