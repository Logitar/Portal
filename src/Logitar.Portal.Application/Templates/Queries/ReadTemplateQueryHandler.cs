using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Queries;

internal class ReadTemplateQueryHandler : IRequestHandler<ReadTemplateQuery, Template?>
{
  private readonly ITemplateQuerier _templateQuerier;

  public ReadTemplateQueryHandler(ITemplateQuerier templateQuerier)
  {
    _templateQuerier = templateQuerier;
  }

  public async Task<Template?> Handle(ReadTemplateQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Template> templates = new(capacity: 2);

    if (query.Id.HasValue)
    {
      Template? template = await _templateQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (template != null)
      {
        templates[template.Id] = template;
      }
    }

    if (query.UniqueName != null)
    {
      Template? template = await _templateQuerier.ReadAsync(query.Realm, query.UniqueName, cancellationToken);
      if (template != null)
      {
        templates[template.Id] = template;
      }
    }

    if (templates.Count > 1)
    {
      throw new TooManyResultsException<Template>(expected: 1, actual: templates.Count);
    }

    return templates.Values.SingleOrDefault();
  }
}
