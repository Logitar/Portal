using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Portal.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers;

internal class InitializeDatabaseCommandHandler : INotificationHandler<InitializeDatabaseCommand>
{
  private readonly IConfiguration _configuration;
  private readonly EventContext _eventContext;
  private readonly PortalContext _portalContext;

  public InitializeDatabaseCommandHandler(IConfiguration configuration, EventContext eventContext, PortalContext portalContext)
  {
    _configuration = configuration;
    _eventContext = eventContext;
    _portalContext = portalContext;
  }

  public async Task Handle(InitializeDatabaseCommand notification, CancellationToken cancellationToken)
  {
    if (_configuration.GetValue<bool>("EnableMigrations"))
    {
      await _eventContext.Database.MigrateAsync(cancellationToken);
      await _portalContext.Database.MigrateAsync(cancellationToken);
    }
  }
}
