using Logitar.Portal.Domain.ApiKeys.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.ApiKeys;

internal class ApiKeyUpdatedEventHandler : INotificationHandler<ApiKeyUpdatedEvent>
{
  private readonly PortalContext _context;

  public ApiKeyUpdatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(ApiKeyUpdatedEvent @event, CancellationToken cancellationToken)
  {
    ApiKeyEntity apiKey = await _context.ApiKeys
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<ApiKeyEntity>(@event.AggregateId);

    HashSet<string> roleIds = @event.Roles.Keys.ToHashSet();
    RoleEntity[] roles = await _context.Roles
      .Where(x => roleIds.Contains(x.AggregateId))
      .ToArrayAsync(cancellationToken);

    apiKey.Update(@event, roles);

    ActorEntity? actor = await _context.Actors
      .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.ToGuid(), cancellationToken);
    if (actor == null)
    {
      actor = new(apiKey);

      _context.Actors.Add(actor);
    }
    else if (@event.Title != null)
    {
      actor.Update(@event);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
