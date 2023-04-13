using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Commands;

internal class MigrateDatabaseHandler : IRequestHandler<MigrateDatabase>
{
  private readonly EventContext _eventContext;
  private readonly PortalContext _portalContext;

  public MigrateDatabaseHandler(EventContext eventContext, PortalContext portalContext)
  {
    _eventContext = eventContext;
    _portalContext = portalContext;
  }

  public async Task Handle(MigrateDatabase request, CancellationToken cancellationToken)
  {
    await _eventContext.Database.MigrateAsync(cancellationToken);
    await _portalContext.Database.MigrateAsync(cancellationToken);
  }
}
