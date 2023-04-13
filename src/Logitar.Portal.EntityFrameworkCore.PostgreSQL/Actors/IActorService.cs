using Logitar.EventSourcing;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Actors;

internal interface IActorService
{
  Task<ActorEntity> GetAsync(DomainEvent e, CancellationToken cancellationToken = default);

  Task DeleteAsync(UserEntity user, CancellationToken cancellationToken = default);
  Task UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);
}
