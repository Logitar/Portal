using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Core.Templates.Queries;

internal record GetTemplates(string? Realm, string? Search, TemplateSort? Sort,
  bool IsDescending, int? Skip, int? Limit) : IRequest<PagedList<Template>>;
