using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Queries;

internal class SearchTemplatesQueryHandler : IRequestHandler<SearchTemplatesQuery, SearchResults<Template>>
{
  private readonly ITemplateQuerier _templateQuerier;

  public SearchTemplatesQueryHandler(ITemplateQuerier templateQuerier)
  {
    _templateQuerier = templateQuerier;
  }

  public async Task<SearchResults<Template>> Handle(SearchTemplatesQuery query, CancellationToken cancellationToken)
  {
    return await _templateQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
