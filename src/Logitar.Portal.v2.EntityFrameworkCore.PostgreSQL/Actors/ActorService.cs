using Logitar.EventSourcing;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Actors;

internal class ActorService : IActorService
{
  public async Task<ActorEntity> GetAsync(DomainEvent e, CancellationToken cancellationToken)
  {
    Guid id = e.ActorId.ToGuid();

    if (id == Guid.Empty)
    {
      return ActorEntity.System;
    }

    throw new NotImplementedException(); // TODO(fpion): user actors
  }
}
