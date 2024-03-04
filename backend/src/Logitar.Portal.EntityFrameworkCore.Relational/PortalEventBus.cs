using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Handlers.ApiKeys;
using Logitar.Identity.EntityFrameworkCore.Relational.Handlers.Passwords;
using Logitar.Identity.EntityFrameworkCore.Relational.Handlers.Roles;
using Logitar.Identity.EntityFrameworkCore.Relational.Handlers.Sessions;
using Logitar.Identity.EntityFrameworkCore.Relational.Handlers.Users;
using Logitar.Portal.Application.Logging;
using MediatR;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal class PortalEventBus : EventBus
{
  private readonly ILoggingService _loggingService;

  public PortalEventBus(ILoggingService loggingService, IPublisher publisher, IApiKeyEventHandler apiKeyEventHandler, IOneTimePasswordEventHandler oneTimePasswordEventHandler, IRoleEventHandler roleEventHandler, ISessionEventHandler sessionEventHandler, IUserEventHandler userEventHandler)
    : base(publisher, apiKeyEventHandler, oneTimePasswordEventHandler, roleEventHandler, sessionEventHandler, userEventHandler)
  {
    _loggingService = loggingService;
  }

  public override async Task PublishAsync(DomainEvent @event, CancellationToken cancellationToken)
  {
    _loggingService.Report(@event);

    await base.PublishAsync(@event, cancellationToken);
  }
}
