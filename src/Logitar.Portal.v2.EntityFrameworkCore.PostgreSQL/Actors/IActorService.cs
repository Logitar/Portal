using Logitar.EventSourcing;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Actors;

internal interface IActorService
{
  Task<ActorEntity> GetAsync(DomainEvent e, CancellationToken cancellationToken = default);
}
