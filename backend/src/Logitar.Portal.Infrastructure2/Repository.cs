using Logitar.Portal.Core2;
using Logitar.Portal.Infrastructure2.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure2
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
