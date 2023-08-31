using Logitar.Portal.Domain.Roles.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Roles;

internal class RoleUpdatedEventHandler : INotificationHandler<RoleUpdatedEvent>
{
  private readonly PortalContext _context;

  public RoleUpdatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(RoleUpdatedEvent @event, CancellationToken cancellationToken)
  {
    RoleEntity role = await _context.Roles
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<RoleEntity>(@event.AggregateId);

    role.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
