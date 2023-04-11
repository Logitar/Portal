using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.v2.Core.Templates.Queries;

internal record GetTemplates(string? Realm, string? Search, TemplateSort? Sort,
  bool IsDescending, int? Skip, int? Limit) : IRequest<PagedList<Template>>;
