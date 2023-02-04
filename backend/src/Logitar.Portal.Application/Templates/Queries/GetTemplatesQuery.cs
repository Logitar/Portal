using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Queries
{
  internal record GetTemplatesQuery(string? Realm, string? Search,
    TemplateSort? Sort, bool IsDescending, int? Index, int? Count) : IRequest<ListModel<TemplateModel>>;
}
