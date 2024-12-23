using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Queries;

internal record SearchTemplatesQuery(SearchTemplatesPayload Payload) : Activity, IRequest<SearchResults<TemplateModel>>;

internal class SearchTemplatesQueryHandler : IRequestHandler<SearchTemplatesQuery, SearchResults<TemplateModel>>
{
  private readonly ITemplateQuerier _templateQuerier;

  public SearchTemplatesQueryHandler(ITemplateQuerier templateQuerier)
  {
    _templateQuerier = templateQuerier;
  }

  public async Task<SearchResults<TemplateModel>> Handle(SearchTemplatesQuery query, CancellationToken cancellationToken)
  {
    return await _templateQuerier.SearchAsync(query.Realm, query.Payload, cancellationToken);
  }
}
