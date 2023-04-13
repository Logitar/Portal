using MediatR;

namespace Logitar.Portal.Core.Templates.Commands;

internal class DeleteTemplatesHandler : IRequestHandler<DeleteTemplates>
{
  private readonly ICurrentActor _currentActor;
  private readonly ITemplateRepository _templateRepository;

  public DeleteTemplatesHandler(ICurrentActor currentActor, ITemplateRepository templateRepository)
  {
    _currentActor = currentActor;
    _templateRepository = templateRepository;
  }

  public async Task Handle(DeleteTemplates request, CancellationToken cancellationToken)
  {
    IEnumerable<TemplateAggregate> templates = await _templateRepository.LoadAsync(request.Realm, cancellationToken);
    foreach (TemplateAggregate template in templates)
    {
      template.Delete(_currentActor.Id);
    }

    await _templateRepository.SaveAsync(templates, cancellationToken);
  }
}
