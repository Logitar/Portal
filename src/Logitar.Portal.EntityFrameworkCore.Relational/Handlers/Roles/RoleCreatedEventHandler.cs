using Logitar.Portal.Domain.Roles.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Roles;

internal class RoleCreatedEventHandler : INotificationHandler<RoleCreatedEvent>
{
  private readonly PortalContext _context;

  public RoleCreatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(RoleCreatedEvent @event, CancellationToken cancellationToken)
  {
    RoleEntity? role = await _context.Roles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (role == null)
    {
      role = new(@event);

      _context.Roles.Add(role);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
