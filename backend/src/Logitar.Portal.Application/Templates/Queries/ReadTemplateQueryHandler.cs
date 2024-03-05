using Logitar.Portal.Contracts;
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
      Template? template = await _templateQuerier.ReadAsync(query.Realm, query.Id.Value, cancellationToken);
      if (template != null)
      {
        templates[template.Id] = template;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueKey))
    {
      Template? template = await _templateQuerier.ReadAsync(query.Realm, query.UniqueKey, cancellationToken);
      if (template != null)
      {
        templates[template.Id] = template;
      }
    }

    if (templates.Count > 1)
    {
      throw new TooManyResultsException<Template>(expectedCount: 1, actualCount: templates.Count);
    }

    return templates.Values.SingleOrDefault();
  }
}
