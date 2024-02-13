using Logitar.Portal.Domain.Dictionaries.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Dictionaries;

internal class DictionaryLocaleChangedEventHandler : INotificationHandler<DictionaryLocaleChangedEvent>
{
  private readonly PortalContext _context;

  public DictionaryLocaleChangedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(DictionaryLocaleChangedEvent @event, CancellationToken cancellationToken)
  {
    DictionaryEntity? dictionary = await _context.Dictionaries
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (dictionary != null)
    {
      dictionary.SetLocale(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
