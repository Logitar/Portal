using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Domain.ApiKeys;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class ApiKeyRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IApiKeyRepository
{
  public ApiKeyRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task SaveAsync(ApiKeyAggregate apiKey, CancellationToken cancellationToken)
    => await base.SaveAsync(apiKey, cancellationToken);
}
