using Portal.Core;
using Portal.Infrastructure.Entities;

namespace Portal.Infrastructure.Repositories
{
  internal class Repository<T> : IRepository<T> where T : Aggregate
  {
    private readonly PortalDbContext _dbContext;

    public Repository(PortalDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task SaveAsync(T aggregate, CancellationToken cancellationToken = default)
    {
      ArgumentNullException.ThrowIfNull(aggregate);

      Save(aggregate);

      await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(IEnumerable<T> aggregates, CancellationToken cancellationToken = default)
    {
      ArgumentNullException.ThrowIfNull(aggregates);

      foreach (T aggregate in aggregates)
      {
        Save(aggregate);
      }

      await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private void Save(T aggregate)
    {
      if (aggregate.HasChanges)
      {
        IEnumerable<Event> events = Event.FromChanges(aggregate);

        _dbContext.Events.AddRange(events);

        aggregate.ClearChanges();
      }

      if (aggregate.IsDeleted)
      {
        _dbContext.Remove(aggregate);
      }
      else if (aggregate.Sid > 0)
      {
        _dbContext.Update(aggregate);
      }
      else
      {
        _dbContext.Add(aggregate);
      }
    }
  }
}
