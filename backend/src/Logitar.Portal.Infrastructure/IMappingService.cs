using Logitar.Portal.Core.Models;
using Logitar.Portal.Infrastructure.Entities;

namespace Logitar.Portal.Infrastructure
{
  internal interface IMappingService
  {
    Task<T> MapAsync<T>(AggregateEntity aggregate, CancellationToken cancellationToken = default) where T : AggregateModel;
    Task<IEnumerable<T>> MapAsync<T>(IEnumerable<AggregateEntity> aggregates, CancellationToken cancellationToken = default) where T : AggregateModel;
  }
}
