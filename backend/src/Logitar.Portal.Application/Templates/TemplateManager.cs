using Logitar.EventSourcing;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Templates.Events;

namespace Logitar.Portal.Application.Templates;

internal class TemplateManager : ITemplateManager
{
  private readonly ITemplateRepository _templateRepository;

  public TemplateManager(ITemplateRepository templateRepository)
  {
    _templateRepository = templateRepository;
  }

  public async Task SaveAsync(TemplateAggregate template, ActorId actorId, CancellationToken cancellationToken)
  {
    bool hasUniqueKeyChanged = false;
    foreach (DomainEvent change in template.Changes)
    {
      if (change is TemplateCreatedEvent || change is TemplateUniqueKeyChangedEvent)
      {
        hasUniqueKeyChanged = true;
      }
    }

    if (hasUniqueKeyChanged)
    {
      TemplateAggregate? other = await _templateRepository.LoadAsync(template.TenantId, template.UniqueKey, cancellationToken);
      if (other?.Equals(template) == false)
      {
        throw new UniqueKeyAlreadyUsedException(template.TenantId, template.UniqueKey);
      }
    }

    await _templateRepository.SaveAsync(template, cancellationToken);
  }
}
