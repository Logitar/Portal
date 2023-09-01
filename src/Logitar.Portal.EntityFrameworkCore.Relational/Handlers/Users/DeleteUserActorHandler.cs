using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.Domain.Users.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Users;

internal class DeleteUserActorHandler : INotificationHandler<UserDeletedEvent>
{
  private readonly IAggregateRepository _aggregateRepository;
  private readonly PortalContext _context;

  public DeleteUserActorHandler(IAggregateRepository aggregateRepository, PortalContext context)
  {
    _aggregateRepository = aggregateRepository;
    _context = context;
  }

  public async Task Handle(UserDeletedEvent @event, CancellationToken cancellationToken)
  {
    ActorEntity? actor = await _context.Actors
      .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.ToGuid(), cancellationToken);
    if (actor == null)
    {
      UserAggregate user = await _aggregateRepository.LoadAsync<UserAggregate>(@event.AggregateId, @event.Version, includeDeleted: true, cancellationToken)
        ?? throw new AggregateNotFoundException<UserAggregate>(@event.AggregateId, nameof(@event.AggregateId));

      actor = new(user);

      _context.Actors.Add(actor);
    }
    else
    {
      actor.Delete(@event);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
