using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class UserRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IUserRepository
{
  public UserRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task SaveAsync(UserAggregate user, CancellationToken cancellationToken)
    => await base.SaveAsync(user, cancellationToken);
}
