using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal record DeleteTemplateCommand(Guid Id) : Activity, IRequest<TemplateModel?>;

internal class DeleteTemplateCommandHandler : IRequestHandler<DeleteTemplateCommand, TemplateModel?>
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

  public async Task<TemplateModel?> Handle(DeleteTemplateCommand command, CancellationToken cancellationToken)
  {
    TemplateId templateId = new(command.TenantId, new EntityId(command.Id));
    Template? template = await _templateRepository.LoadAsync(templateId, cancellationToken);
    if (template == null || template.TenantId != command.TenantId)
    {
      return null;
    }
    TemplateModel result = await _templateQuerier.ReadAsync(command.Realm, template, cancellationToken);

    ActorId actorId = command.ActorId;
    template.Delete(actorId);
    await _templateManager.SaveAsync(template, actorId, cancellationToken);

    return result;
  }
}
