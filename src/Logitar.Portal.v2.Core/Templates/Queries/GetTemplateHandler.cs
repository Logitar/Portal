using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.v2.Core.Templates.Queries;

internal class GetTemplateHandler : IRequestHandler<GetTemplate, Template?>
{
  private readonly ITemplateQuerier _templateQuerier;

  public GetTemplateHandler(ITemplateQuerier templateQuerier)
  {
    _templateQuerier = templateQuerier;
  }

  public async Task<Template?> Handle(GetTemplate request, CancellationToken cancellationToken)
  {
    List<Template> templates = new(capacity: 2);

    if (request.Id.HasValue)
    {
      templates.AddIfNotNull(await _templateQuerier.GetAsync(request.Id.Value, cancellationToken));
    }
    if (request.Realm != null && request.Key != null)
    {
      templates.AddIfNotNull(await _templateQuerier.GetAsync(request.Realm, request.Key, cancellationToken));
    }

    if (templates.Count > 1)
    {
      throw new TooManyResultsException();
    }

    return templates.SingleOrDefault();
  }
}
