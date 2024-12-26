﻿using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Queries;

internal class ReadTemplateQueryHandler : IRequestHandler<ReadTemplateQuery, TemplateModel?>
{
  private readonly ITemplateQuerier _templateQuerier;

  public ReadTemplateQueryHandler(ITemplateQuerier templateQuerier)
  {
    _templateQuerier = templateQuerier;
  }

  public async Task<TemplateModel?> Handle(ReadTemplateQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, TemplateModel> templates = new(capacity: 2);

    if (query.Id.HasValue)
    {
      TemplateModel? template = await _templateQuerier.ReadAsync(query.Realm, query.Id.Value, cancellationToken);
      if (template != null)
      {
        templates[template.Id] = template;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueKey))
    {
      TemplateModel? template = await _templateQuerier.ReadAsync(query.Realm, query.UniqueKey, cancellationToken);
      if (template != null)
      {
        templates[template.Id] = template;
      }
    }

    if (templates.Count > 1)
    {
      throw new TooManyResultsException<TemplateModel>(expectedCount: 1, actualCount: templates.Count);
    }

    return templates.Values.SingleOrDefault();
  }
}
