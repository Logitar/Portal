using Logitar.Portal.Domain.Roles.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Roles;

internal class RoleDeletedEventHandler : INotificationHandler<RoleDeletedEvent>
{
  private readonly PortalContext _context;

  public RoleDeletedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(RoleDeletedEvent @event, CancellationToken cancellationToken)
  {
    RoleEntity? role = await _context.Roles
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (role != null)
    {
      _context.Roles.Remove(role);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
