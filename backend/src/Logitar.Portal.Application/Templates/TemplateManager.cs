using Logitar.EventSourcing;
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

  public async Task SaveAsync(Template template, ActorId actorId, CancellationToken cancellationToken)
  {
    bool hasUniqueKeyChanged = false;
    foreach (IEvent change in template.Changes)
    {
      if (change is TemplateCreated || change is TemplateUniqueKeyChanged)
      {
        hasUniqueKeyChanged = true;
      }
    }

    if (hasUniqueKeyChanged)
    {
      Template? other = await _templateRepository.LoadAsync(template.TenantId, template.UniqueKey, cancellationToken);
      if (other?.Equals(template) == false)
      {
        throw new UniqueKeyAlreadyUsedException(template.TenantId, template.UniqueKey);
      }
    }

    await _templateRepository.SaveAsync(template, cancellationToken);
  }
}
