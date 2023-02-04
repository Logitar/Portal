using Logitar.Portal.Domain.Dictionaries.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Dictionaries
{
  internal class DictionaryCreatedEventHandler : INotificationHandler<DictionaryCreatedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<DictionaryCreatedEventHandler> _logger;

    public DictionaryCreatedEventHandler(PortalContext context, ILogger<DictionaryCreatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(DictionaryCreatedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        RealmEntity? realm = null;
        if (notification.RealmId.HasValue)
        {
          realm = await _context.Realms.SingleOrDefaultAsync(x => x.AggregateId == notification.RealmId.Value.Value, cancellationToken);
          if (realm == null)
          {
            _logger.LogError("The dictionary 'AggregateId={aggregateId}' could not be found.", notification.RealmId);

            return;
          }
        }

        Actor actor = await _context.GetActorAsync(notification.ActorId, cancellationToken);
        DictionaryEntity dictionary = new(notification, actor, realm);

        _context.Dictionaries.Add(dictionary);

        await _context.SaveChangesAsync(cancellationToken);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
