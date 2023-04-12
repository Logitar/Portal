using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.v2.Core.Templates.Queries;

internal class GetTemplatesHandler : IRequestHandler<GetTemplates, PagedList<Template>>
{
  private readonly ITemplateQuerier _templateQuerier;

  public GetTemplatesHandler(ITemplateQuerier templateQuerier)
  {
    _templateQuerier = templateQuerier;
  }

  public async Task<PagedList<Template>> Handle(GetTemplates request, CancellationToken cancellationToken)
  {
    return await _templateQuerier.GetAsync(request.Realm, request.Search, request.Sort,
      request.IsDescending, request.Skip, request.Limit, cancellationToken);
  }
}
