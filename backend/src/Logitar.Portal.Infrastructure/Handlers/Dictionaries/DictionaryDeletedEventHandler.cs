using Logitar.Portal.Domain.Dictionaries.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Dictionaries
{
  internal class DictionaryDeletedEventHandler : INotificationHandler<DictionaryDeletedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<DictionaryDeletedEventHandler> _logger;

    public DictionaryDeletedEventHandler(PortalContext context, ILogger<DictionaryDeletedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(DictionaryDeletedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        DictionaryEntity? dictionary = await _context.Dictionaries
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (dictionary != null)
        {
          _context.Dictionaries.Remove(dictionary);

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
