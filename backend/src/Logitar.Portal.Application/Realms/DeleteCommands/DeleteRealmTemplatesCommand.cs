using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Realms.DeleteCommands;

internal record DeleteRealmTemplatesCommand(Realm Realm, ActorId ActorId) : INotification;

internal class DeleteRealmTemplatesCommandHandler : INotificationHandler<DeleteRealmTemplatesCommand>
{
  private readonly ITemplateRepository _templateRepository;

  public DeleteRealmTemplatesCommandHandler(ITemplateRepository templateRepository)
  {
    _templateRepository = templateRepository;
  }

  public async Task Handle(DeleteRealmTemplatesCommand command, CancellationToken cancellationToken)
  {
    TenantId tenantId = new(command.Realm.Id.Value);
    IEnumerable<Template> templates = await _templateRepository.LoadAsync(tenantId, cancellationToken);

    foreach (Template template in templates)
    {
      template.Delete(command.ActorId);
    }

    await _templateRepository.SaveAsync(templates, cancellationToken);
  }
}
