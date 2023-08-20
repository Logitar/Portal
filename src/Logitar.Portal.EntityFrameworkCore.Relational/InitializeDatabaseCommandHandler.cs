using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal class InitializeDatabaseCommandHandler : INotificationHandler<InitializeDatabaseCommand>
{
  private readonly EventContext _eventContext;
  private readonly IdentityContext _identityContext;
  private readonly PortalContext _portalContext;

  public InitializeDatabaseCommandHandler(EventContext eventContext,
    IdentityContext identityContext, PortalContext portalContext)
  {
    _eventContext = eventContext;
    _identityContext = identityContext;
    _portalContext = portalContext;
  }

  public async Task Handle(InitializeDatabaseCommand notification, CancellationToken cancellationToken)
  {
    await _eventContext.Database.MigrateAsync(cancellationToken);
    await _identityContext.Database.MigrateAsync(cancellationToken);
    await _portalContext.Database.MigrateAsync(cancellationToken);
  }
}
