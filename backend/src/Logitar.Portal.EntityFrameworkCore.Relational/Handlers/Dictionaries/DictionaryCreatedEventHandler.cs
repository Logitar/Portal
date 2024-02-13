using Logitar.Portal.Domain.Dictionaries.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Dictionaries;

internal class DictionaryCreatedEventHandler : INotificationHandler<DictionaryCreatedEvent>
{
  private readonly PortalContext _context;

  public DictionaryCreatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(DictionaryCreatedEvent @event, CancellationToken cancellationToken)
  {
    DictionaryEntity? dictionary = await _context.Dictionaries.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (dictionary == null)
    {
      dictionary = new(@event);

      _context.Dictionaries.Add(dictionary);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
