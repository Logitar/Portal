using Logitar.Portal.Domain.Dictionaries.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Dictionaries
{
  internal class DictionaryUpdatedEventHandler : INotificationHandler<DictionaryUpdatedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<DictionaryUpdatedEventHandler> _logger;

    public DictionaryUpdatedEventHandler(PortalContext context, ILogger<DictionaryUpdatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(DictionaryUpdatedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        DictionaryEntity? dictionary = await _context.Dictionaries
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (dictionary == null)
        {
          _logger.LogError("The API key 'AggregateId={aggregateId}' could not be found.", notification.AggregateId);
        }
        else
        {
          dictionary.Update(notification);

          await _context.SaveChangesAsync(cancellationToken);
        }
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
