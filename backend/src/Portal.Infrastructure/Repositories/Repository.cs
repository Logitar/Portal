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

      if (aggregate.HasChanges)
      {
        IEnumerable<EventEntity> events = EventEntity.FromChanges(aggregate);

        _dbContext.Events.AddRange(events);
        await _dbContext.SaveChangesAsync(cancellationToken);

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

      await _dbContext.SaveChangesAsync(cancellationToken);
    }
  }
}
