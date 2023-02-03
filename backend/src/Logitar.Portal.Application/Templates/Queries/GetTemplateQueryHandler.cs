using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Queries
{
  internal class GetTemplateQueryHandler : IRequestHandler<GetTemplateQuery, TemplateModel?>
  {
    private readonly ITemplateQuerier _templateQuerier;

    public GetTemplateQueryHandler(ITemplateQuerier templateQuerier)
    {
      _templateQuerier = templateQuerier;
    }

    public async Task<TemplateModel?> Handle(GetTemplateQuery request, CancellationToken cancellationToken)
    {
      return await _templateQuerier.GetAsync(request.Id, cancellationToken);
    }
  }
}
