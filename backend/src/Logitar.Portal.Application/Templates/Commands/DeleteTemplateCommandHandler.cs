using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal class DeleteTemplateCommandHandler : IRequestHandler<DeleteTemplateCommand, Template?>
{
  private readonly ITemplateManager _templateManager;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public DeleteTemplateCommandHandler(ITemplateManager templateManager, ITemplateQuerier templateQuerier, ITemplateRepository templateRepository)
  {
    _templateManager = templateManager;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<Template?> Handle(DeleteTemplateCommand command, CancellationToken cancellationToken)
  {
    TemplateAggregate? template = await _templateRepository.LoadAsync(command.Id, cancellationToken);
    if (template == null || template.TenantId != command.TenantId)
    {
      return null;
    }
    Template result = await _templateQuerier.ReadAsync(command.Realm, template, cancellationToken);

    ActorId actorId = command.ActorId;
    template.Delete(actorId);
    await _templateManager.SaveAsync(template, actorId, cancellationToken);

    return result;
  }
}
