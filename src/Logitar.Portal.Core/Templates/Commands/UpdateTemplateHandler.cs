using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Core.Templates.Commands;

internal class UpdateTemplateHandler : IRequestHandler<UpdateTemplate, Template>
{
  private readonly ICurrentActor _currentActor;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public UpdateTemplateHandler(ICurrentActor currentActor,
    ITemplateQuerier templateQuerier,
    ITemplateRepository templateRepository)
  {
    _currentActor = currentActor;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<Template> Handle(UpdateTemplate request, CancellationToken cancellationToken)
  {
    TemplateAggregate template = await _templateRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<TemplateAggregate>(request.Id);

    UpdateTemplateInput input = request.Input;

    template.Update(_currentActor.Id, input.Subject, input.ContentType, input.Contents,
      input.DisplayName, input.Description);

    await _templateRepository.SaveAsync(template, cancellationToken);

    return await _templateQuerier.GetAsync(template, cancellationToken);
  }
}
