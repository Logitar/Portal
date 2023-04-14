﻿using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Core.Templates.Commands;

internal class UpdateTemplateHandler : IRequestHandler<UpdateTemplate, Template>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public UpdateTemplateHandler(IApplicationContext applicationContext,
    ITemplateQuerier templateQuerier,
    ITemplateRepository templateRepository)
  {
    _applicationContext = applicationContext;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<Template> Handle(UpdateTemplate request, CancellationToken cancellationToken)
  {
    TemplateAggregate template = await _templateRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<TemplateAggregate>(request.Id);

    UpdateTemplateInput input = request.Input;

    template.Update(_applicationContext.ActorId, input.Subject, input.ContentType, input.Contents,
      input.DisplayName, input.Description);

    await _templateRepository.SaveAsync(template, cancellationToken);

    return await _templateQuerier.GetAsync(template, cancellationToken);
  }
}
