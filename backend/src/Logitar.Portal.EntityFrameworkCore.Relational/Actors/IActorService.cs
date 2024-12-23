using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Actors;

internal interface IActorService
{
  Task<IEnumerable<ActorModel>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
}
