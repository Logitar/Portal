﻿using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Core.Templates.Commands;

internal class DeleteTemplateHandler : IRequestHandler<DeleteTemplate, Template>
{
  private readonly ICurrentActor _currentActor;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public DeleteTemplateHandler(ICurrentActor currentActor,
    ITemplateQuerier templateQuerier,
    ITemplateRepository templateRepository)
  {
    _currentActor = currentActor;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<Template> Handle(DeleteTemplate request, CancellationToken cancellationToken)
  {
    TemplateAggregate template = await _templateRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<TemplateAggregate>(request.Id);
    Template output = await _templateQuerier.GetAsync(template, cancellationToken);

    // TODO(fpion): set null if used as password recovery template in Realm?

    template.Delete(_currentActor.Id);

    await _templateRepository.SaveAsync(template, cancellationToken);

    return output;
  }
}
