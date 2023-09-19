using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal class DeleteTemplatesCommandHandler : INotificationHandler<DeleteTemplatesCommand>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ITemplateRepository _templateRepository;

  public DeleteTemplatesCommandHandler(IApplicationContext applicationContext, ITemplateRepository templateRepository)
  {
    _applicationContext = applicationContext;
    _templateRepository = templateRepository;
  }

  public async Task Handle(DeleteTemplatesCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<TemplateAggregate> templates = await _templateRepository.LoadAsync(command.Realm, cancellationToken);
    foreach (TemplateAggregate template in templates)
    {
      template.Delete(_applicationContext.ActorId);
    }

    await _templateRepository.SaveAsync(templates, cancellationToken);
  }
}
