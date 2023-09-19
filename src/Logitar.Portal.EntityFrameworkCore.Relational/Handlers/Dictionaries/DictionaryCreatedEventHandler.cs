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
    DictionaryEntity? dictonary = await _context.Dictionaries.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (dictonary == null)
    {
      dictonary = new(@event);

      _context.Dictionaries.Add(dictonary);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
