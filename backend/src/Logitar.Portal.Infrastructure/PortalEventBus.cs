using Logitar.EventSourcing;
using Logitar.Identity.Infrastructure;
using Logitar.Identity.Infrastructure.Handlers;
using Logitar.Portal.Application.Logging;
using MediatR;

namespace Logitar.Portal.Infrastructure;

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
