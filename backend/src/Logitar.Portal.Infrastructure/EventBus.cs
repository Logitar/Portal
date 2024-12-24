using Logitar.EventSourcing;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Logging;
using MediatR;

namespace Logitar.Portal.Infrastructure;

internal class EventBus : Identity.Infrastructure.EventBus
{
  private readonly ICacheService _cacheService;
  private readonly ILoggingService _loggingService;

  public EventBus(ICacheService cacheService, ILoggingService loggingService, IMediator mediator) : base(mediator)
  {
    _cacheService = cacheService;
    _loggingService = loggingService;
  }

  public override async Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
  {
    await base.PublishAsync(@event, cancellationToken);

    if (@event is DomainEvent domainEvent)
    {
      _loggingService.Report(domainEvent);

      string? @namespace = domainEvent.GetType().Namespace;
      switch (@namespace)
      {
        case "Logitar.Identity.Core.ApiKeys.Events":
        case "Logitar.Identity.Core.Users.Events":
          ActorId actorId = new(domainEvent.StreamId.Value);
          _cacheService.RemoveActor(actorId);
          break;
      }
    }
  }
}
