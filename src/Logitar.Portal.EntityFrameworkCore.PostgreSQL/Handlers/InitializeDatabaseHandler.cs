using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.Infrastructure.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Handlers;

internal class InitializeDatabaseHandler : INotificationHandler<InitializeDatabase>
{
  private readonly PortalContext _portalContext;
  private readonly IConfiguration _configuration;
  private readonly EventContext _eventContext;

  public InitializeDatabaseHandler(IConfiguration configuration,
    EventContext eventContext,
    PortalContext portalContext)
  {
    _configuration = configuration;
    _eventContext = eventContext;
    _portalContext = portalContext;
  }

  public async Task Handle(InitializeDatabase notification, CancellationToken cancellationToken)
  {
    if (_configuration.GetValue<bool>("MigrateDatabase"))
    {
      await _portalContext.Database.MigrateAsync(cancellationToken);
      await _eventContext.Database.MigrateAsync(cancellationToken);
    }
  }
}
