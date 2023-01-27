using Logitar.Portal.Core;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure
{
  internal class Repository : IRepository
  {
    private readonly PortalContext _context;
    private readonly IPublisher _publisher;

    public Repository(PortalContext context, IPublisher publisher)
    {
      _context = context;
      _publisher = publisher;
    }

    public async Task<T?> LoadAsync<T>(AggregateId id, CancellationToken cancellationToken) where T : AggregateRoot
    {
      string aggregateType = typeof(T).GetName();

      EventEntity[] events = await _context.Events.AsNoTracking()
        .Where(x => x.AggregateType == aggregateType && x.AggregateId == id.ToString())
        .OrderBy(x => x.Version)
        .ToArrayAsync(cancellationToken);

      if (!events.Any())
      {
        return null;
      }

      return AggregateRoot.LoadFromHistory<T>(events.Select(e => e.Deserialize()), id);
    }

    public async Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<AggregateId> ids, CancellationToken cancellationToken) where T : AggregateRoot
    {
      string aggregateType = typeof(T).GetName();
      IEnumerable<string> idValues = ids.Select(x => x.Value);

      EventEntity[] events = await _context.Events.AsNoTracking()
        .Where(x => x.AggregateType == aggregateType && idValues.Contains(x.AggregateId))
        .OrderBy(x => x.Version)
        .ToArrayAsync(cancellationToken);

      if (!events.Any())
      {
        return Enumerable.Empty<T>();
      }

      return events.GroupBy(x => x.AggregateId)
        .Select(g => AggregateRoot.LoadFromHistory<T>(g.Select(e => e.Deserialize()), new AggregateId(g.Key)));
    }

    public async Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken)
    {
      if (aggregate.HasChanges)
      {
        IEnumerable<EventEntity> events = EventEntity.FromChanges(aggregate);
        _context.Events.AddRange(events);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (DomainEvent change in aggregate.Changes)
        {
          await _publisher.Publish(change, cancellationToken);
        }

        aggregate.ClearChanges();
      }
    }

    public async Task SaveAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken)
    {
      IEnumerable<EventEntity> events = aggregates.SelectMany(EventEntity.FromChanges);
      if (events.Any())
      {
        _context.Events.AddRange(events);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (AggregateRoot aggregate in aggregates)
        {
          if (aggregate.HasChanges)
          {
            foreach (DomainEvent change in aggregate.Changes)
            {
              await _publisher.Publish(change, cancellationToken);
            }

            aggregate.ClearChanges();
          }
        }
      }
    }
  }
}
