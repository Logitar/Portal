using Logitar.Portal.Domain.Dictionaries.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Dictionaries;

internal class DictionaryDeletedEventHandler : INotificationHandler<DictionaryDeletedEvent>
{
  private readonly PortalContext _context;

  public DictionaryDeletedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(DictionaryDeletedEvent @event, CancellationToken cancellationToken)
  {
    DictionaryEntity? dictionary = await _context.Dictionaries
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (dictionary != null)
    {
      _context.Dictionaries.Remove(dictionary);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
