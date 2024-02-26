using Logitar.EventSourcing;
using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries;

public interface IDictionaryManager
{
  Task SaveAsync(DictionaryAggregate dictionary, ActorId actorId = default, CancellationToken cancellationToken = default);
}
