using Logitar.EventSourcing;
using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Application.Templates;

public interface ITemplateManager
{
  Task SaveAsync(Template template, ActorId actorId = default, CancellationToken cancellationToken = default);
}
