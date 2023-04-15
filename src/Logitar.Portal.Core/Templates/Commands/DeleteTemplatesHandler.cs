using MediatR;

namespace Logitar.Portal.Core.Templates.Commands;

internal class DeleteTemplatesHandler : IRequestHandler<DeleteTemplates>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ITemplateRepository _templateRepository;

  public DeleteTemplatesHandler(IApplicationContext applicationContext, ITemplateRepository templateRepository)
  {
    _applicationContext = applicationContext;
    _templateRepository = templateRepository;
  }

  public async Task Handle(DeleteTemplates request, CancellationToken cancellationToken)
  {
    IEnumerable<TemplateAggregate> templates = await _templateRepository.LoadAsync(request.Realm, cancellationToken);
    foreach (TemplateAggregate template in templates)
    {
      template.Delete(_applicationContext.ActorId);
    }

    await _templateRepository.SaveAsync(templates, cancellationToken);
  }
}
