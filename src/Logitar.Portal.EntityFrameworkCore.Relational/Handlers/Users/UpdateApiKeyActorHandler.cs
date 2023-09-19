using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.Domain.Users.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Users;

internal class UpdateUserActorHandler : INotificationHandler<UserUpdatedEvent>
{
  private readonly IAggregateRepository _aggregateRepository;
  private readonly PortalContext _context;

  public UpdateUserActorHandler(IAggregateRepository aggregateRepository, PortalContext context)
  {
    _aggregateRepository = aggregateRepository;
    _context = context;
  }

  public async Task Handle(UserUpdatedEvent @event, CancellationToken cancellationToken)
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
    else if (@event.UniqueName != null || @event.FullName != null || @event.Email != null || @event.Picture != null)
    {
      UserAggregate user = await _aggregateRepository.LoadAsync<UserAggregate>(@event.AggregateId, @event.Version, includeDeleted: true, cancellationToken)
        ?? throw new AggregateNotFoundException<UserAggregate>(@event.AggregateId, nameof(@event.AggregateId));

      actor.Update(user);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
