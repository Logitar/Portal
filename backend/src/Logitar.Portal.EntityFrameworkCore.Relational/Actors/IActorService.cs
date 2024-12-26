using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Actors;

internal interface IActorService
{
  Task<IReadOnlyCollection<ActorModel>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
}
