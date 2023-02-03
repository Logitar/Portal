using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Queries
{
  internal class GetTemplatesQueryHandler : IRequestHandler<GetTemplatesQuery, ListModel<TemplateModel>>
  {
    private readonly ITemplateQuerier _templateQuerier;

    public GetTemplatesQueryHandler(ITemplateQuerier templateQuerier)
    {
      _templateQuerier = templateQuerier;
    }

    public async Task<ListModel<TemplateModel>> Handle(GetTemplatesQuery request, CancellationToken cancellationToken)
    {
      return await _templateQuerier.GetPagedAsync(request.Realm, request.Search,
        request.Sort, request.IsDescending,
        request.Index, request.Count,
        cancellationToken);
    }
  }
}
