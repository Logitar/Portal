using Logitar.Portal.Application;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Repositories
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

    public async Task<T?> LoadAsync<T>(string id, CancellationToken cancellationToken) where T : AggregateRoot
      => await LoadAsync<T>(new AggregateId(id), cancellationToken);
    public async Task<T?> LoadAsync<T>(AggregateId id, CancellationToken cancellationToken) where T : AggregateRoot
    {
      string aggregateType = typeof(T).GetName();

      IEnumerable<EventEntity> events = await _context.Events.AsNoTracking()
        .Where(x => x.AggregateType == aggregateType && x.AggregateId == id.Value)
        .OrderBy(x => x.Version)
        .ToArrayAsync(cancellationToken);

      if (!events.Any())
      {
        return null;
      }

      return AggregateRoot.LoadFromHistory<T>(events.Select(e => e.Deserialize()), id);
    }

    public async Task<Configuration?> LoadConfigurationAsync(CancellationToken cancellationToken)
    {
      return await LoadAsync<Configuration>(Configuration.AggregateId, cancellationToken);
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
