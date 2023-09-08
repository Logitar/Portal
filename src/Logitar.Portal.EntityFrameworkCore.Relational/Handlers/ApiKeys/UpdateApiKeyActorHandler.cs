using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.ApiKeys.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.ApiKeys;

internal class UpdateApiKeyActorHandler : INotificationHandler<ApiKeyUpdatedEvent>
{
  private readonly IAggregateRepository _aggregateRepository;
  private readonly PortalContext _context;

  public UpdateApiKeyActorHandler(IAggregateRepository aggregateRepository, PortalContext context)
  {
    _aggregateRepository = aggregateRepository;
    _context = context;
  }

  public async Task Handle(ApiKeyUpdatedEvent @event, CancellationToken cancellationToken)
  {
    ActorEntity? actor = await _context.Actors
      .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.ToGuid(), cancellationToken);
    if (actor == null)
    {
      ApiKeyAggregate apiKey = await _aggregateRepository.LoadAsync<ApiKeyAggregate>(@event.AggregateId, @event.Version, includeDeleted: true, cancellationToken)
        ?? throw new AggregateNotFoundException<ApiKeyAggregate>(@event.AggregateId, nameof(@event.AggregateId));

      actor = new(apiKey);

      _context.Actors.Add(actor);
    }
    else if (@event.DisplayName != null)
    {
      actor.Update(@event);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
