using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Infrastructure.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers;

internal class InitializeDatabaseCommandHandler : INotificationHandler<InitializeDatabaseCommand>
{
  private readonly bool _enableMigrations;
  private readonly EventContext _eventContext;
  private readonly IdentityContext _identityContext;
  private readonly PortalContext _portalContext;

  public InitializeDatabaseCommandHandler(IConfiguration configuration, EventContext eventContext, IdentityContext identityContext, PortalContext portalContext)
  {
    _enableMigrations = configuration.GetValue<bool>("EnableMigrations");
    _eventContext = eventContext;
    _identityContext = identityContext;
    _portalContext = portalContext;
  }

  public async Task Handle(InitializeDatabaseCommand _, CancellationToken cancellationToken)
  {
    if (_enableMigrations)
    {
      await _eventContext.Database.MigrateAsync(cancellationToken);
      await _identityContext.Database.MigrateAsync(cancellationToken);
      await _portalContext.Database.MigrateAsync(cancellationToken);
    }
  }
}
